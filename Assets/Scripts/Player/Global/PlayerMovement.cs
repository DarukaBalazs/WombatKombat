using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Component References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Player settings")]
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpForce = 15;

    [Header("Grounding")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private float horizontal;

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocityY);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }
 
}
