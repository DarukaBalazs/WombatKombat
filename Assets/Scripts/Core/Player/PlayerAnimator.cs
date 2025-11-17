using UnityEngine;

/// <summary>
/// Az állapotgépből, a BaseMovement-ből és a Rigidbody sebességéből
/// frissíti az Animator paramétereit:
/// - Idle / Run / Jump / Jumping / JumpToFall / Falling / Landing
/// - WallSlide / WallJump
/// </summary>
[RequireComponent(typeof(PlayerStateManager))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerStateManager state;
    [SerializeField] BaseMovement movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;

    [Header("Locomotion thresholds")]
    [SerializeField] float runSpeedThreshold = 0.1f;    // vízszintes sebesség, ami fölött futás
    [SerializeField] float runStopDelay = 0.1f;     // ennyi ideig lehet lassú, mielőtt leáll a futás
    [SerializeField] float fallSpeedThreshold = -0.1f;  // ez alatt már esésnek számít

    bool wasGroundedLastFrame;
    bool isRunning;
    float runStopTimer;

    void Awake()
    {
        if (!state) state = GetComponent<PlayerStateManager>();
        if (!movement) movement = GetComponent<BaseMovement>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        if (state != null)
            state.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        if (state != null)
            state.OnStateChanged -= HandleStateChanged;
    }

    void Update()
    {
        if (!animator || !state || !rb) return;

        bool grounded = state.IsGrounded;
        Vector2 vel = rb.linearVelocity;
        float speedX = Mathf.Abs(vel.x);
        float speedY = vel.y;

        bool wallSliding = movement != null && movement.IsWallSliding;

        // Alap paraméterek
        animator.SetBool("IsGrounded", grounded);
        animator.SetBool("IsWallSliding", wallSliding);
        animator.SetFloat("Speed", speedX);
        animator.SetFloat("VerticalSpeed", speedY);

        // Futás logika időküszöbbel – NE fusson/álljon le 1 frame spike-ra
        bool shouldRunNow = grounded && speedX > runSpeedThreshold && !wallSliding;

        if (shouldRunNow)
        {
            isRunning = true;
            runStopTimer = 0f;
        }
        else
        {
            if (isRunning)
            {
                runStopTimer += Time.deltaTime;
                if (runStopTimer >= runStopDelay)
                    isRunning = false;
            }
        }

        animator.SetBool("IsRunning", isRunning);

        // Esés flag – csak ha nem wallslide
        bool isFalling = !grounded && speedY < fallSpeedThreshold && !wallSliding;
        animator.SetBool("IsFalling", isFalling);

        // Landing trigger – levegőből földre érkezés
        if (!wasGroundedLastFrame && grounded)
        {
            animator.SetTrigger("Land");
        }

        wasGroundedLastFrame = grounded;
    }

    /// <summary>
    /// Állapotváltásokra reagálunk triggerekrel:
    /// Jump (felugrás) és WallJump.
    /// </summary>
    void HandleStateChanged(State previous, State current)
    {
        if (!animator) return;

        switch (current)
        {
            case State.Jump:
                animator.SetTrigger("Jump");
                break;

            case State.WallJump:
                animator.SetTrigger("WallJump");
                break;
        }
    }
}
