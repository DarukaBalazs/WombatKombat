using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Player settings")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 5;

    [Header("Grounding")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    float coyoteTime = 0.2f;
    float coyoteCounter;
    float jumpBufferTime = 0.2f;
    float jumpBufferCounter;
    bool canCancelJump = false;


    private float horizontal;

    private void Update()
    {
        TurnTheRightWay();
        
        if (IsGrounded()) { coyoteCounter = coyoteTime; canCancelJump = true; } else {  coyoteCounter -= Time.deltaTime; }
        jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocityY);
        if (jumpBufferCounter > 0f && coyoteCounter > 0f) { canCancelJump = true; rb.linearVelocityY = jumpForce; }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        } else if (context.canceled && !IsGrounded() && canCancelJump)
        {
            rb.linearVelocityY = rb.linearVelocityY * 0.5f;
            canCancelJump = false;
        }
    }

    private void TurnTheRightWay()
    {
        if (horizontal > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontal < -0.01)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }
 

}
