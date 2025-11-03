using System;
using System.Collections.Generic;
using UnityEngine;
using Combat;
public enum State
{
    Idle,
    Run,
    Jump,
    WallJump,
    Fall,
    Attacking,
    Hitstun,
    Dead
}

public class PlayerStateManager : MonoBehaviour
{
    #region Field
    #region SerializeFields
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [Header("Current states")]
    [SerializeField] State currentState = State.Idle;
    #endregion
    private bool isGrounded;
    private bool isWallSliding;
    private bool isAttacking;
    private bool hasAirAttacked;
    private bool isStunned;
    private bool isDead;
    private bool isInvulnerable;
    private bool didDoubleJump;

    private AttackPhase currentAttackPhase = AttackPhase.None;
    private float attackPhaseTimer;   // visszaszámláló a fázishoz
    private float hitstunTimer;       // stun ideje
    private float invulnTimer;        // invuln ideje

    private readonly Dictionary<State, int> priorities =
    new Dictionary<State, int>
    {
            { State.Idle, 0 },
            { State.Run, 1 },
            { State.Jump, 2 },
            { State.WallJump, 2 },
            { State.Fall, 2 },
            { State.Attacking, 4 },
            { State.Hitstun, 5 },
            { State.Dead, 6 }
    };

    private readonly HashSet<State> lockedStates = new()
    {
        State.Attacking,
        State.Hitstun,
        State.Dead
    };
    #endregion

    #region Events
    public event Action<State, State> OnStateChanged;
    #endregion

    #region Property
    public bool IsGrounded => isGrounded; 
    public bool IsWallSliding => isWallSliding; 
    public bool IsAttacking => isAttacking;
    public bool HasAirAttacked => hasAirAttacked;
    public bool IsStunned => isStunned;
    public bool IsDead => isDead;
    public bool IsInvulnerable => isInvulnerable;
    public bool DidDoubleJump => didDoubleJump;
    #endregion

    #region Gate
    public bool CanMove() => !isAttacking && !isStunned && !isDead;

    public bool CanJump() => (isGrounded || isWallSliding || !didDoubleJump) && !isAttacking && !isStunned && !isDead;

    public bool CanAirAttack() => !isGrounded && !hasAirAttacked && !isStunned && !isDead;

    public bool CanAttack() => !isAttacking && !hasAirAttacked && !isStunned && !isDead;

    public bool CanSlide() => !isAttacking && !isStunned && !isDead;
    #endregion

    #region Life-cycle
    public void Initialize(Animator animator, Rigidbody2D rb)
    {
        if (this.animator == null) this.animator = animator;
        if (this.rb == null) this.rb = rb;
    }

    public void Tick(float dt)
    {
        UpdateTimers(dt);

        // egyszerű automata visszaváltások mozgásállapotokra (ha nincs lockolt state)
        if (!lockedStates.Contains(currentState))
        {
            // groundon → Idle/Run választást bízd a movementre,
            // itt csak példamutató fallback:
            if (isGrounded && (IsIn(State.Fall) || IsIn(State.Jump) || IsIn(State.WallJump)))
                RequestTransition(State.Idle);

            else if (!isGrounded && IsIn(State.Idle))
                RequestTransition(State.Fall);
        }
    }


    public void FixedTick(float dt)
    {
        // ide kerülhet később knockdown/fizikai-recovery logika, ha kell
    }

    private void UpdateTimers(float dt)
    {
        // Attack fázis időzítés: Runner hívhatná, de ha itt tartod, akkor lejáratkor léptethetsz
        if (isAttacking && attackPhaseTimer > 0f)
        {
            attackPhaseTimer -= dt;
            if (attackPhaseTimer <= 0f)
            {
                // ha lejárt, NEM itt döntünk a következő fázisról,
                // azt a Runner hívja újabb EnterAttackPhase-szel, vagy ha Recovery után vagyunk: ExitAttack()
                attackPhaseTimer = 0f;
            }
        }

        // Hitstun
        if (isStunned && hitstunTimer > 0f)
        {
            hitstunTimer -= dt;
            if (hitstunTimer <= 0f)
                ClearHitstun();
        }

        // Invuln
        if (isInvulnerable && invulnTimer > 0f)
        {
            invulnTimer -= dt;
            if (invulnTimer <= 0f)
            {
                isInvulnerable = false;
                invulnTimer = 0f;
            }
        }
    }

    #endregion

    #region Transitions 
    public bool RequestTransition(State target)
    {
        if (target == currentState) return false;

        if (CanLeaveCurrentState(target))
        {
            ChangeState(target);
            return true;
        }

        return false;
    }

    public void ForceTransition(State target)
    {
        if (target == currentState)
            return;

        ChangeState(target);
    }
    private bool CanLeaveCurrentState(State target)
    { 
        if (lockedStates.Contains(currentState))
        {
            return priorities[target] >= priorities[currentState];
        }
        return true;
    }

    private void ChangeState(State next)
    {
        State prev = currentState;
        currentState = next;

        Debug.Log($"State changed: {prev} → {next}");

        OnStateChanged?.Invoke(prev, next);
    }

    public bool IsIn(State s) => currentState == s;

    public void ForceKill()
    {
        ForceTransition(State.Dead);
    }
    #endregion
    #region Environment
    public void OnGroundedChanged(bool grounded)
    {
        if (isGrounded == grounded) return;
        isGrounded = grounded;

        if (grounded)
        {
            // landoláskor levegős ütés reset
            hasAirAttacked = false;
            ResetDoubleJump();

            // ha esésből érkeztünk, álljunk Idle/Run-ba (hagyjuk a Movementre a döntést)
            if (IsIn(State.Fall) || IsIn(State.Jump) || IsIn(State.WallJump))
                RequestTransition(State.Idle);
        }
    }
    public void SetWallSliding(bool wallSliding)
    {
        isWallSliding = wallSliding;
    }
    #endregion
    #region Attack Control
    public void EnterAttackPhase(AttackPhase phase, float phaseDuration)
    {
        currentAttackPhase = phase;
        attackPhaseTimer = Mathf.Max(0f, phaseDuration);

        if (!isAttacking)
        {
            isAttacking = true;
            // lockolt állapotba váltunk
            RequestTransition(State.Attacking);

            // levegős indításkor jelöld elhasználtnak
            if (!isGrounded && !hasAirAttacked)
                hasAirAttacked = true;
        }

        // opcionális: animátor jelzők itt (ha már be van kötve)
        // animator?.SetBool("IsAttacking", true);
        // animator?.SetInteger("AttackPhase", (int)currentAttackPhase);
    }

    public void ExitAttack()
    {
        isAttacking = false;
        currentAttackPhase = AttackPhase.None;
        attackPhaseTimer = 0f;

        // HA magasabb prioritású lockban vagyunk, ne váltsunk (stun/dead).
        if (isDead || isStunned || IsIn(State.Hitstun) || IsIn(State.Dead))
            return;

        // Csak akkor kényszeríts vissza locomotion-be, ha tényleg Attackingból jövünk.
        if (IsIn(State.Attacking))
        {
            if (isGrounded)
                ForceTransition(State.Idle);
            else
                ForceTransition(State.Fall);
        }
    }
    #endregion
    #region Status effects
    public void EnterHitstun(float seconds)
    {
        isStunned = true;
        hitstunTimer = Mathf.Max(hitstunTimer, seconds);

        // támadás megszakítása policy szerint
        if (isAttacking)
            ExitAttack();

        RequestTransition(State.Hitstun);
        // animator?.SetBool("IsStunned", true);
    }

    public void EnterInvulnerable(float seconds)
    {
        isInvulnerable = true;
        invulnTimer = Mathf.Max(invulnTimer, seconds);
    }

    private void ClearHitstun()
    {
        isStunned = false;
        hitstunTimer = 0f;
        // animator?.SetBool("IsStunned", false);

        // vissza locomotion-be
        if (isGrounded) RequestTransition(State.Idle);
        else RequestTransition(State.Fall);
    }
    #endregion

    #region Double jump
    public void MarkDoubleJumpUsed()
    {
        didDoubleJump = true;
    }

    public void ResetDoubleJump()
    {
        didDoubleJump = false;
    }
    #endregion
}
