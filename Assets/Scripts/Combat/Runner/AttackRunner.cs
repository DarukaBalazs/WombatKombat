using UnityEngine;
using Combat;
using System.Collections;
using System.Collections.Generic;

public class AttackRunner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerStateManager state;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] InputHandling input;
    [SerializeField] HitboxGroup hitboxGroup;

    public InputHandling Input => input;
    public HitboxGroup Hitboxes => hitboxGroup;
    public PlayerStateManager State => state;
    public Animator Animator => animator;
    public Rigidbody2D Body => rb;

    private Coroutine running;
    private float cooldownUntil;
    private AttackAsset currentAsset;

    public bool TryStart(AttackAsset asset)
    {
        if (asset == null) return false;
        if (Time.time < cooldownUntil) return false;

        bool grounded = state != null && state.IsGrounded;
        if (asset.requiresGrounded && !grounded) return false;

        bool canAttack = grounded
            ? state.CanAttack()
            : (asset.allowAir && state.CanAirAttack());

        if (!canAttack) return false;

        if (running != null)
            StopCoroutine(running);

        running = StartCoroutine(Run(asset));
        return true;
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

        if (state != null)
            state.SetAttackLocks(asset.lockMovement, asset.lockJump);

        EnterPhase(AttackPhase.StartUp, asset);
        yield return Wait(asset.windup);

        EnterPhase(AttackPhase.Active, asset);
        yield return StartCoroutine(RunActiveTimeline(asset));

        EnterPhase(AttackPhase.Recovery, asset);
        yield return Wait(asset.recovery);

        ExitAttack(asset);
        currentAsset = null;
    }

    private void EnterPhase(AttackPhase phase, AttackAsset asset)
    {
        if (state == null) return;

        float phaseDur =
            phase == AttackPhase.StartUp ? asset.windup :
            phase == AttackPhase.Active ? asset.active :
            phase == AttackPhase.Recovery ? asset.recovery : 0f;

        state.EnterAttackPhase(phase, phaseDur);

        // ide jön majd az animáció váltás
    }

    private void ExitAttack(AttackAsset asset)
    {
        cooldownUntil = Time.time + Mathf.Max(0f, asset.cooldown);

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

            // cancelOnLanding: landoláskor az összes aktív step OnExit, ACTIVE vége
            if (asset.cancelOnLanding && state != null)
            {
                bool grounded = state.IsGrounded;
                if (!wasGrounded && grounded)
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
            }

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