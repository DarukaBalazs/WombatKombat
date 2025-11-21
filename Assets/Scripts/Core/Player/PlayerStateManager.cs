using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    #region Field-ek
    private enum CurrentState
    {
        Idle,
        Run,
        Jump,
        WallJump,
        Fall,
        Attacking,
        HitStun,
        Dead
    }
    private bool isGrounded;
    private bool isWallSliding;
    private bool isAttacking;
    private bool hasAirAttacked;
    private bool isStunned;
    private bool isDead;
    private bool isInvulnerable;
    #endregion

    #region Property-k
    public bool IsGrounded => isGrounded; 
    public bool IsWallSliding => isWallSliding; 
    public bool IsAttacking => isAttacking;
    public bool HasAirAttacked => hasAirAttacked;
    public bool IsStunned => isStunned;
    public bool IsDead => isDead;
    public bool IsInvulnerable => isInvulnerable;
    #endregion

    #region Gate-ek
    public bool CanMove() => !isAttacking && !isStunned && !isDead;

    public bool CanJump() => (isGrounded || isWallSliding) && !isAttacking && !isStunned && !isDead;

    public bool CanAirAttack() => !isGrounded && !hasAirAttacked && !isStunned && !isDead;

    public bool CanAttck() => !isAttacking && !HasAirAttacked && !isStunned && !isDead;
    #endregion


}
