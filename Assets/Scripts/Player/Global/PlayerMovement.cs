using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Private SerializeFields
    [Header("Player Component References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerInput input;

    [Header("Player settings")]
    [SerializeField] float speed = 5;
    [SerializeField] float jumpForce = 5;

    [Header("Grounding")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    [Header("Wall jumping")]
    [SerializeField] Transform wallCheck;
    [SerializeField] float wallJumpSpeed = 5;
    [SerializeField] float wallSlideSpeed = 2f;
    #endregion

    #region Private fields
    //jumping variables
    float coyoteTime = 0.2f;
    float coyoteCounter;
    float jumpBufferTime = 0.2f;
    float jumpBufferCounter;
    bool canCancelJump = false;

    //input variables
    InputAction moveAction;
    InputAction jumpAction;

    //movement variables
    float horizontal;

    //wallslide variables
    bool isSliding;
    #endregion

    #region Public fields

    #endregion

    private void Update()
    {
        TurnTheRightWay();
        
        if (IsGrounded()) { coyoteCounter = coyoteTime; canCancelJump = true; } else {  coyoteCounter -= Time.deltaTime; }
        jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);

        if (IsWallTouch() && !IsGrounded() && horizontal != 0)
        {
            isSliding = true;
        } else
        {
            isSliding = false;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocityY);
        if (jumpBufferCounter > 0f && coyoteCounter > 0f) { canCancelJump = true; rb.AddForce(new Vector2(0, jumpForce / 5), ForceMode2D.Impulse); }
        if (isSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX,Mathf.Clamp(rb.linearVelocityY, -wallSlideSpeed,float.MaxValue));
        }
    }

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
        }
        if (context.canceled && canCancelJump && rb.linearVelocityY > 0f)
        {
            rb.linearVelocityY = rb.linearVelocityY * 0.5f;
            canCancelJump = false;
        }
    }
    #endregion

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
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.5f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

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
