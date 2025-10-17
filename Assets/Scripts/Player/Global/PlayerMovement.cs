using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerMovement : MonoBehaviour
{
    #region Protected SerializeFields
    [Header("Player Component References")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected PlayerInput input;

    [Header("Player settings")]
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float jumpForce = 5f;
    [SerializeField] protected float doubleJumpMultiplier = 0.7f;

    [Header("Grounding")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Transform groundCheck;

    [Header("Wall jumping")]
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallJumpPowerX = 7f;
    [SerializeField] protected float wallJumpPowerY = 15f;
    [SerializeField] protected float wallJumpDuration = 0.4f;
    [SerializeField] protected float wallSlideSpeed = 2f;
    #endregion

    #region Protected fields
    //jumping variables
    protected float coyoteTime = 0.2f;
    protected float coyoteCounter;
    protected float jumpBufferTime = 0.2f;
    protected float jumpBufferCounter;
    protected bool canCancelJump = false;
    protected bool canDoubleJump = false;

    //wall jumping variables
    protected bool isWallJumping;
    protected float wallJumpDirection;
    //input variables
    protected InputAction moveAction;
    protected InputAction jumpAction;

    //movement variables
    protected float horizontal;

    //wallslide variables
    protected bool isSliding;

    //animator variables
    protected bool faceRight;
    #endregion

    #region Public fields

    #endregion

    #region Update methods
    protected virtual void Update()
    {
        if (!isWallJumping)
            TurnTheRightWay();

        #region Jumping
        if (IsGrounded()) 
        {
            coyoteCounter = coyoteTime;
            canCancelJump = true; 
            canDoubleJump = true;
        } else
        {
            coyoteCounter -= Time.deltaTime;
        }
        jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);
        if (isSliding) canDoubleJump = true;    
        #endregion

        #region Wall sliding
        isSliding = !isWallJumping && IsWallTouch() && !IsGrounded() && horizontal != 0;
        #endregion
    }

    protected virtual void FixedUpdate()
    {
        if (!isWallJumping) rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocityY);
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            canCancelJump = true;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpBufferCounter = 0f;
        }

        if (jumpBufferCounter > 0f && !IsGrounded() && canDoubleJump && !isWallJumping)
        {
            canCancelJump = true;
            float doubleJumpForce;
            if (rb.linearVelocityY < 0f) doubleJumpForce = jumpForce * 2f * doubleJumpMultiplier;
            else doubleJumpForce = jumpForce * doubleJumpMultiplier;

            rb.AddForce(new Vector2(0, doubleJumpForce), ForceMode2D.Impulse);
            canDoubleJump = false;
            jumpBufferCounter = 0f;
        }
        if (isSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX,Mathf.Clamp(rb.linearVelocityY, -wallSlideSpeed,float.MaxValue));
        }
    }   
    #endregion

    #region Start, OnEnable, OnDisable
    protected void Start()
    {
        #region Input map setting
        var map = input.currentActionMap;

        moveAction = map.FindAction("Move");
        jumpAction = map.FindAction("Jump");

        moveAction.performed += Move;
        moveAction.canceled += Move;

        jumpAction.performed += Jump;
        jumpAction.canceled += Jump;

        map.Enable();
        #endregion
    }

    protected void OnDisable()
    {
        #region Input map setting
        moveAction.performed -= Move;
        moveAction.canceled -= Move;

        jumpAction.performed -= Jump;
        jumpAction.canceled -= Jump;
        #endregion
    }
    #endregion

    #region Events from Input System
    public virtual void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public virtual void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;

            if (isSliding)
            {
                StartWallJump();
            }
        }
        if (context.canceled && canCancelJump && rb.linearVelocityY > 0f)
        {
            rb.linearVelocityY *= 0.5f;
            canCancelJump = false;
        }
    }
    #endregion

    #region Protected methods

    #region Flipping the sprite
    protected void Flip(float targetDirection)
    {
        if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(targetDirection))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            faceRight = !faceRight;
        }
    }
    protected void TurnTheRightWay()
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
    #endregion

    #region Checkers
    protected bool IsGrounded() => Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    protected bool IsWallTouch() => Physics2D.OverlapBox(wallCheck.position, new Vector2(0.16f, 1.2f), 0, groundLayer);
    #endregion

    protected void StartWallJump()
    {
        wallJumpDirection = -Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(wallJumpDirection*wallJumpPowerX, wallJumpPowerY);

        Flip(wallJumpDirection);

        StopAllCoroutines();
        StartCoroutine(WallJump());
    }

    #region Routines
    protected IEnumerator WallJump()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }
    #endregion
    #endregion

    #region Public methods
    public virtual void ApplyCharacterData(CharacterData data)
    {
        speed = data.baseSpeed;
        jumpForce = data.baseJumpForce;
    }

    #endregion

    #region Prebuilt methods
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.darkRed;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
        Gizmos.DrawCube(wallCheck.position, new Vector2(0.15f, 1.2f));
    }
    #endregion

}
