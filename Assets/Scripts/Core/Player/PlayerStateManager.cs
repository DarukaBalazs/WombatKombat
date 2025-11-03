using System;
using System.Collections.Generic;
using UnityEngine;
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
    #endregion

    #region Gate
    public bool CanMove() => !isAttacking && !isStunned && !isDead;

    public bool CanJump() => (isGrounded || isWallSliding) && !isAttacking && !isStunned && !isDead;

    public bool CanAirAttack() => !isGrounded && !hasAirAttacked && !isStunned && !isDead;

    public bool CanAttck() => !isAttacking && !HasAirAttacked && !isStunned && !isDead;
    #endregion

    #region Life-cycle
    public void Initialize(Animator animator, Rigidbody2D rb)
    {
        if (this.animator == null) this.animator = animator;
        if (this.rb == null) this.rb = rb;
    }

    public void Tick(float dt)
    {

    }

    public void FixedTick(float dt)
    {

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
    #endregion

    #region Attack Controll

    #endregion
}
