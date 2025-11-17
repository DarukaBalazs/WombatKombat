using System.Collections;
using UnityEngine;

/// <summary>
/// A karakter mozg�s��rt, ugr�s��rt, falcs�sz�s��rt �s falugr�s��rt felel�s komponens.
/// K�l�n kezeli a coyote time-ot, jump buffert �s a duplaugr�st.
/// </summary>
public class BaseMovement : MonoBehaviour
{
    [Header("Player settings")]
    [SerializeField] PlayerController controller;
    [SerializeField] PlayerStateManager state;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float doubleJumpMultiplier = 0.7f;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    [Header("Wall jumping")]
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallJumpPowerX = 7f;
    [SerializeField] float wallJumpPowerY = 15f;
    [SerializeField] float wallJumpDuration = 0.4f;
    [SerializeField] float wallSlideSpeed = 2f;

    [Header("Locomotion thresholds")]
    [SerializeField] float runInputThreshold = 0.1f;     // mekkora input/sebess�gn�l sz�m�t fut�snak
    [SerializeField] float idleStateDelay = 0.1f;     // ennyi ideig kell "�ll�s" �llapotban lennie, hogy Idle-re v�ltsunk
    [SerializeField] float fallVelocityThreshold = -0.1f; // ez alatt m�r es�snek vessz�k (Y sebess�g)

    

    #region Private fields
    //jumping variables
    float coyoteTime = 0.2f;
    float coyoteCounter;
    float jumpBufferTime = 0.2f;
    float jumpBufferCounter;
    bool canCancelJump = false;
    bool canDoubleJump = false;

    //wall jumping variables
    bool isWallJumping;
    float wallJumpDirection;

    //movement variables
    [HideInInspector] public float horizontal;

    //Environment variables
    bool isSliding;
    bool isGrounded;
    public bool IsWallSliding => isSliding;

    //animator variables
    bool faceRight;

    float idleTimer; // ennyi ideje "�ll" a karakter
    #endregion

    #region Update methods
    public void Tick(float dt)
    {
        if (!isWallJumping)
            TurnTheRightWay();

        #region Jumping
        state.OnGroundedChanged(isGrounded);
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            canCancelJump = true;
            canDoubleJump = true;
        }
        else
        {
            coyoteCounter -= dt;
        }

        jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - dt);
        if (isSliding) canDoubleJump = true;
        #endregion

        #region Wall sliding
        isSliding = !isWallJumping && IsWallTouch() && !isGrounded && horizontal != 0;
        #endregion

        // �J: locomotion state friss�t�s (Idle / Run / Fall)
        UpdateLocomotionState(dt);
    }
    public void FixedTick(float dt)
    {
        bool groundedNow = IsGrounded();
        if (groundedNow != isGrounded)
        {
            isGrounded = groundedNow;
            state.OnGroundedChanged(isGrounded);
        }
        if (state.CanMove())  Move();
        if (state.CanJump())  Jump();
        if (state.CanSlide()) Slide();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// V�zszintes mozg�s a megadott input alapj�n.
    /// </summary>
    private void Move()
    {
        
        if (!isWallJumping) controller.Rb.linearVelocity = new Vector2(horizontal * speed, controller.Rb.linearVelocityY); 
    }

    /// <summary>
    /// �llapot v�lt�sok kezel�se mozg�s alapj�n:
    /// - f�ld�n: Idle / Run
    /// - leveg�ben: Fall (ha lefel� megy)
    /// </summary>
    private void UpdateLocomotionState(float dt)
    {
        // Ha am�gy sem mozoghat (t�mad�s, stun, halott), ne piszk�ljuk a locomotion state-et
        if (!state.CanMove())
            return;

        bool grounded = isGrounded;
        Vector2 vel = controller.Rb.linearVelocity;
        float velX = Mathf.Abs(vel.x);
        float velY = vel.y;

        // Leveg�ben
        if (!grounded)
        {
            // Ha m�r lefel� megy�nk, es�s �llapot
            if (velY < fallVelocityThreshold)
            {
                state.RequestTransition(State.Fall);
            }

            // Leveg�ben nem kezel�nk Idle/Run-t
            idleTimer = 0f;
            return;
        }

        // F�ld�n: Idle / Run
        float absInput = Mathf.Abs(horizontal);

        // Ha van �rtelmezhet� input vagy sebess�g ? Run
        if (absInput > runInputThreshold || velX > runInputThreshold)
        {
            idleTimer = 0f;
            state.RequestTransition(State.Run);
        }
        else
        {
            // Nincs input: egy kis k�sleltet�s ut�n v�ltson Idle-re
            idleTimer += dt;
            if (idleTimer >= idleStateDelay)
            {
                state.RequestTransition(State.Idle);
            }
        }
    }
    /// <summary>
    /// A karakter sprite ir�ny�nak megford�t�sa mozg�s ir�ny szerint.
    /// </summary>
    private void TurnTheRightWay()
    {
        if (horizontal > 0.01f)
        {
            Flip(1f);
        }
        else if (horizontal < -0.01f)
        {
            Flip(-1f);
        }
    }

    /// <summary>
    /// Kezeli az ugr�s logik�j�t: f�ldi, dupla �s falugr�sokat.
    /// </summary>
    private void Jump()
    {
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            if (state.RequestTransition(State.Jump))
            {
                controller.Rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                canCancelJump = true;
                jumpBufferCounter = 0f;
            }
        }

        if (jumpBufferCounter > 0f && !IsGrounded() && canDoubleJump && !isWallJumping)
        {
            canCancelJump = true;
            float doubleJumpForce;
            if (controller.Rb.linearVelocityY < 0f) doubleJumpForce = jumpForce * 2f * doubleJumpMultiplier;
            else doubleJumpForce = jumpForce * doubleJumpMultiplier;

            controller.Rb.AddForce(new Vector2(0, doubleJumpForce), ForceMode2D.Impulse);
            state.RequestTransition(State.Jump);
            canDoubleJump = false;
            state.MarkDoubleJumpUsed();
            jumpBufferCounter = 0f;
        }
    }

    /// <summary>
    /// Falcs�sz�s lass�t�sa, amikor a j�t�kos a fal mellett lefele mozog.
    /// </summary>
    private void Slide()
    {
        if (isSliding)
        {
            state.RequestTransition(State.Fall);
            state.SetWallSliding(true);
            controller.Rb.linearVelocity = new Vector2(controller.Rb.linearVelocityX, Mathf.Clamp(controller.Rb.linearVelocityY, -wallSlideSpeed, float.MaxValue));
        } else
        {
            state.SetWallSliding(false);
        }
    }

    /// <summary>
    /// Sprite ir�ny�nak megford�t�sa.
    /// </summary>
    private void Flip(float targetDirection)
    {
        // Ha a jelenlegi sk�l�val nem egyezik a k�v�nt ir�ny
        if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(targetDirection))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            faceRight = !faceRight;
        }
    }

    /// <summary>
    /// Falugr�s elind�t�sa, ir�nyv�lt�ssal �s ideiglenes input tilt�ssal.
    /// </summary>
    private void StartWallJump()
    {
        wallJumpDirection = -Mathf.Sign(transform.localScale.x);
        controller.Rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPowerX, wallJumpPowerY);

        Flip(wallJumpDirection);

        StopAllCoroutines();
        StartCoroutine(WallJump());
    }

    #region Checkers
    private bool IsGrounded() => Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0f, groundLayer);

    private bool IsWallTouch() => Physics2D.OverlapBox(wallCheck.position, new Vector2(0.16f, 1.2f), 0f, groundLayer);
    #endregion

    #endregion

    #region Public methods
    /// <summary>
    /// H�vja meg, ha mozg�s input �rkezett a j�t�kost�l.
    /// </summary>
    public void HandleMove(float input)
    {
        horizontal = input;
    }

    /// <summary>
    /// Ugr�s el�k�sz�t�se (jump buffer �s falugr�s logika kezel�se).
    /// </summary>
    public void HandleJump()
    {
        jumpBufferCounter = jumpBufferTime;
        if (isSliding && state.CanJump())
        {
            state.RequestTransition(State.WallJump);
            StartWallJump();
        }
    }

    /// <summary>
    /// Ugr�s megszak�t�sa (ha a j�t�kos elengedi a gombot).
    /// </summary>
    public void CancelJump()
    {
        if (canCancelJump && controller.Rb.linearVelocityY > 0f)
        {
            controller.Rb.linearVelocityY *= 0.5f;
            canCancelJump = false;
        }
    }

    public void ApplyCharacterData(CharacterData data)
    {
        speed = data.baseSpeed;
        jumpForce = data.baseJumpForce;
    }

    #endregion


    #region Coroutines
    private IEnumerator WallJump()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkRed;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
        Gizmos.DrawCube(wallCheck.position, new Vector2(0.15f, 1.2f));
    }
}
