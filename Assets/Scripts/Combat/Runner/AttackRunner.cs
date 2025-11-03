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
    public InputHandling Input => input;

    private Coroutine running;
    private float cooldownUntil;

    public bool TryStart(AttackAsset asset)
    {
        if (asset == null) return false;

        if (Time.time < cooldownUntil) return false;

        bool grounded = state != null && state.IsGrounded;
        if (asset.requiresGrounded && !grounded) return false;

        bool canAttack = grounded ? state.CanAttack() : (asset.allowAir && state.CanAirAttack());
        if (!canAttack) return false;

        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Run(asset));
        return true;
    }

    public void CancelCurrent()
    {
        if (running != null)
        {
            StopCoroutine(running);
            running = null;

            if (state != null) state.ExitAttack();
        }
    }

    private IEnumerator Run(AttackAsset asset)
    {
        EnterPhase(AttackPhase.StartUp, asset);
        yield return Wait(asset.windup);

        EnterPhase(AttackPhase.Active, asset);
        yield return StartCoroutine(RunActiveTimeline(asset));

        if (asset.steps != null) foreach (var step in asset.steps) step.OnExit(this);
        

        EnterPhase(AttackPhase.Recovery, asset);
        yield return Wait(asset.recovery);

        ExitAttack(asset);
    }

    private void EnterPhase(AttackPhase phase, AttackAsset asset)
    {
        if (state != null)
        {
            float phaseDur =
                phase == AttackPhase.StartUp ? asset.windup :
                phase == AttackPhase.Active ? asset.active :
                phase == AttackPhase.Recovery ? asset.recovery : 0f;

            state.EnterAttackPhase(phase, phaseDur);
        }

        //Ide jön majd az animáció
    }

    private void ExitAttack(AttackAsset asset)
    {
        cooldownUntil = Time.time + Mathf.Max(0f, asset.cooldown);

        if (state != null) state.ExitAttack();

        running = null;
    }
    private IEnumerator RunActiveTimeline(AttackAsset asset)
    {
        if (asset.steps == null || asset.steps.Count == 0)
        {
            yield return Wait(asset.active);
            yield break;
        }

        // idõrend: startTime szerint
        var steps = new List<AttackStep>(asset.steps);
        steps.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        float t = 0f;
        int i = 0;

        // belépések idõben
        while (t < asset.active)
        {
            float dt = Time.deltaTime;
            t += dt;
            // köv. step belépési ideje
            for (int k = 0; k < i; k++)
            {
                var s = steps[k];
                if (s != null && t >= s.startTime && t <= s.endTime)
                    s.Tick(this, t, dt);
            }

            // tick

            yield return null;
        }

        // kilépések: minden step, amelynek endTime <= active vége (vagy nincs megadva)
        foreach (var s in steps)
        {
            s.OnExit(this);
        }
    }

    private static YieldInstruction Wait(float seconds)
    {
        return seconds > 0f ? new WaitForSeconds(seconds) : null;
    }

    public PlayerStateManager State => state;
    public Animator Animator => animator;
    public Rigidbody2D Body => rb;
}
