using UnityEngine;
using Combat;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackRunner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerStateManager state;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] InputHandling input;
    [SerializeField] HitboxGroup hitboxGroup;

    [Header("Properties")]
    public InputHandling Input => input;
    public HitboxGroup Hitboxes => hitboxGroup;
    public PlayerStateManager State => state;
    public Animator Animator => animator;
    public Rigidbody2D Body => rb;

    [Header("States")]
    public bool LandedThisAttack { get; private set; }
    public bool HitConnectedThisAttack { get ; private set; }

    private Coroutine running;
    private AttackAsset currentAsset;

    public event Action<AttackAsset> OnAttackStarted;

    private readonly Dictionary<AttackAsset, float> cooldowns = new();

    public bool TryStart(AttackAsset asset)
    {
        if (asset == null) return false;
        if (cooldowns.TryGetValue(asset, out float until) && Time.time < until)
            return false;

        bool grounded = state != null && state.IsGrounded;
        if (asset.requiresGrounded && !grounded) return false;

        bool canAttack = grounded
            ? state.CanAttack()
            : (asset.allowAir && state.CanAirAttack());

        if (!canAttack) return false;

        if (running != null)
            StopCoroutine(running);

        running = StartCoroutine(Run(asset));
        animator.SetTrigger(asset.type.ToString()+"Pressed");
        return true;
    }

    private void StartCooldown(AttackAsset asset)
    {
        if (asset == null || asset.cooldown <= 0f) return;
        cooldowns[asset] = Time.time + asset.cooldown;
    }
    public void MarkHitConnected()
    {
        HitConnectedThisAttack = true;
    } 
    public void CancelCurrent()
    {
        // minden step OnExit
        if (currentAsset != null && currentAsset.steps != null)
        {
            foreach (var step in currentAsset.steps)
            {
                if (step != null)
                    step.OnExit(this);
            }
        }

        if (running != null)
        {
            StopCoroutine(running);
            running = null;
        }

        if (state != null)
        {
            state.ClearAttackLocks();
            state.ExitAttack();
        }
    }

    private IEnumerator Run(AttackAsset asset)
    {
        currentAsset = asset;
        LandedThisAttack = false;
        HitConnectedThisAttack = false;
        
        OnAttackStarted?.Invoke(asset);

        if (state != null)
            state.SetAttackLocks(asset.lockMovement, asset.lockJump);

        EnterPhase(AttackPhase.StartUp, asset);

        float t = 0f;
        while (t < asset.windup)
        {
            if (asset.stopMovementOnWindup && rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            t += Time.deltaTime;
            yield return null;
        }

        EnterPhase(AttackPhase.Active, asset);
        yield return StartCoroutine(RunActiveTimeline(asset));

        EnterPhase(AttackPhase.Recovery, asset);
        yield return Wait(asset.recovery);

        ExitAttack(asset);
        currentAsset = null;
    }

    private void EnterPhase(AttackPhase phase, AttackAsset asset)
    {
        if (asset.lockMovement &&
            phase == AttackPhase.StartUp &&
            state != null &&
            state.IsGrounded &&
            rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (state != null)
        {
            float phaseDur =
                phase == AttackPhase.StartUp ? asset.windup :
                phase == AttackPhase.Active ? asset.active :
                phase == AttackPhase.Recovery ? asset.recovery : 0f;

            state.EnterAttackPhase(phase, phaseDur);
        }

        if (phase == AttackPhase.StartUp && asset.stopMovementOnWindup && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ExitAttack(AttackAsset asset)
    {
        StartCooldown(asset);

        if (state != null)
        {
            state.ClearAttackLocks();
            state.ExitAttack();
        }


        running = null;
    }

    private IEnumerator RunActiveTimeline(AttackAsset asset)
    {
        if (asset.steps == null || asset.steps.Count == 0)
        {
            yield return Wait(asset.active);
            yield break;
        }

        // idõrend szerint
        var sorted = new List<AttackStep>(asset.steps);
        sorted.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        float activeTime = Mathf.Max(0f, asset.active);
        float t = 0f;
        int nextIndex = 0;
        var activeSteps = new List<AttackStep>();

        bool wasGrounded = state != null && state.IsGrounded;

        while (t < activeTime)
        {
            float dt = Time.deltaTime;
            t += dt;

            bool grounded = state != null && state.IsGrounded;
            bool justLanded = !wasGrounded && grounded;

            if (justLanded)
            {
                LandedThisAttack = true;
            }

            // cancelOnLanding: landoláskor az összes aktív step OnExit, ACTIVE vége
            if (asset.cancelOnLanding && justLanded)
            {

                for (int i = 0; i < activeSteps.Count; i++)
                {
                    var s = activeSteps[i];
                    if (s != null) s.OnExit(this);
                }
                activeSteps.Clear();
                yield break;
                
            }
            wasGrounded = grounded;
            

            // új step-ek beléptetése
            while (nextIndex < sorted.Count && t >= sorted[nextIndex].startTime)
            {
                var s = sorted[nextIndex];
                if (s != null)
                {
                    s.Setup(gameObject);
                    s.OnEnter(this);
                    activeSteps.Add(s);
                }
                nextIndex++;
            }

            // aktív step-ek Tick / idõalapú kilépés
            for (int i = activeSteps.Count - 1; i >= 0; i--)
            {
                var s = activeSteps[i];
                if (s == null)
                {
                    activeSteps.RemoveAt(i);
                    continue;
                }

                if (t > s.endTime)
                {
                    s.OnExit(this);
                    activeSteps.RemoveAt(i);
                }
                else
                {
                    s.Tick(this, t, dt);
                }
            }

            yield return null;
        }

        // ACTIVE végén minden bent maradt step kilép
        for (int i = 0; i < activeSteps.Count; i++)
        {
            var s = activeSteps[i];
            if (s != null) s.OnExit(this);
        }
    }

    private static YieldInstruction Wait(float seconds)
    {
        return seconds > 0f ? new WaitForSeconds(seconds) : null;
    }
}