using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Private SerializeFields
    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerInput input;

    [Header("Player settings")]
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
    #endregion

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
    //input variables
    InputAction moveAction;
    InputAction jumpAction;

    //movement variables
    float horizontal;

    //wallslide variables
    bool isSliding;

    //animator variables
    bool faceRight;
    #endregion


    #region Public fields

    #endregion

    #region Update methods
    private void Update()
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

    void FixedUpdate()
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
    private void Start()
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

    private void OnDisable()
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
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
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

    #region Private methods

    #region Flipping the sprite
    private void Flip(float targetDirection)
    {
        // Ha a jelenlegi skálával nem egyezik a kívánt irány
        if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(targetDirection))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            faceRight = !faceRight;
        }
    }
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
    #endregion

    #region Checkers
    private bool IsGrounded() => Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    private bool IsWallTouch() => Physics2D.OverlapBox(wallCheck.position, new Vector2(0.16f, 1.2f), 0, groundLayer);
    #endregion

    private void StartWallJump()
    {
        wallJumpDirection = -Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(wallJumpDirection*wallJumpPowerX, wallJumpPowerY);

        Flip(wallJumpDirection);

        StopAllCoroutines();
        StartCoroutine(WallJump());
    }

    #region Routines
    private IEnumerator WallJump()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }
    #endregion
    #endregion

    #region Public methods
    public void ApplyCharacterData(CharacterData data)
    {
        speed = data.baseSpeed;
        jumpForce = data.baseJumpForce;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkRed;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
        Gizmos.DrawCube(wallCheck.position, new Vector2(0.15f, 1.2f));
    }

}
