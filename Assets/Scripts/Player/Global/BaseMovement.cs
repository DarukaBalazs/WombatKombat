using System.Collections;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    [Header("Player settings")]
    [SerializeField] CharacterController controller;
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
    float horizontal;

    //wallslide variables
    bool isSliding;

    //animator variables
    bool faceRight;
    #endregion

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
        }
        else
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
        Move();
        Jump();
        Slide();
    }

    private void Move()
    {
        if (!isWallJumping) controller.Rb.linearVelocity = new Vector2(horizontal * speed, controller.Rb.linearVelocityY);
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

    private void Jump()
    {
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            canCancelJump = true;
            controller.Rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpBufferCounter = 0f;
        }

        if (jumpBufferCounter > 0f && !IsGrounded() && canDoubleJump && !isWallJumping)
        {
            canCancelJump = true;
            float doubleJumpForce;
            if (controller.Rb.linearVelocityY < 0f) doubleJumpForce = jumpForce * 2f * doubleJumpMultiplier;
            else doubleJumpForce = jumpForce * doubleJumpMultiplier;

            controller.Rb.AddForce(new Vector2(0, doubleJumpForce), ForceMode2D.Impulse);
            canDoubleJump = false;
            jumpBufferCounter = 0f;
        }
    }

    private void Slide()
    {
        if (isSliding)
        {
            controller.Rb.linearVelocity = new Vector2(controller.Rb.linearVelocityX, Mathf.Clamp(controller.Rb.linearVelocityY, -wallSlideSpeed, float.MaxValue));
        }
    }

    public void HandleMove(float h)
    {
        horizontal = h;
    }

    public void HandleJump()
    {
        jumpBufferCounter = jumpBufferTime;
        if (isSliding)
        {
            StartWallJump();
        }
    }

    public void CancelJump()
    {
        if (canCancelJump && controller.Rb.linearVelocityY > 0f)
        {
            controller.Rb.linearVelocityY *= 0.5f;
            canCancelJump = false;
        }
    }

    private void StartWallJump()
    {
        wallJumpDirection = -Mathf.Sign(transform.localScale.x);
        controller.Rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPowerX, wallJumpPowerY);

        Flip(wallJumpDirection);

        StopAllCoroutines();
        StartCoroutine(WallJump());
    }

    private IEnumerator WallJump()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }

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
    private bool IsGrounded() => Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);

    private bool IsWallTouch() => Physics2D.OverlapBox(wallCheck.position, new Vector2(0.16f, 1.2f), 0, groundLayer);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkRed;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
        Gizmos.DrawCube(wallCheck.position, new Vector2(0.15f, 1.2f));
    }

    public void ApplyCharacterData(CharacterData data)
    {
        speed = data.baseSpeed;
        jumpForce = data.baseJumpForce;
    }
}
