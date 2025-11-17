using UnityEngine;

/// <summary>
/// Az állapotgépből és a Rigidbody sebességből frissíti az Animator paramétereit.
/// Animációk: Idle, Run, Jump, Jumping, JumpToFall, Falling, Landing.
/// </summary>
[RequireComponent(typeof(PlayerStateManager))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerStateManager state;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;

    [Header("Thresholds")]
    [SerializeField] float runSpeedThreshold = 0.1f;   // ekkora vízszintes sebesség fölött számít futásnak
    [SerializeField] float fallSpeedThreshold = -0.1f; // ez alatt már esésnek számít
    [SerializeField] float runStopDelay = 0.1f;    // ennyi ideig lehet "lassú", mielőtt leállna a futás

    bool wasGroundedLastFrame;

    // ÚJ: futás állapot belső tárolása + timer
    bool isRunning;
    float runStopTimer;


    void Awake()
    {
        if (!state) state = GetComponent<PlayerStateManager>();
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
        if (!animator || !state || !rb)
            return;

        bool grounded = state.IsGrounded;
        Vector2 vel = rb.linearVelocity;
        float speedX = Mathf.Abs(vel.x);
        float speedY = vel.y;

        // Alap paraméterek
        animator.SetBool("IsGrounded", grounded);
        animator.SetFloat("Speed", speedX);        // Idle/Run blendhez
        animator.SetFloat("VerticalSpeed", speedY); // Jumping / JumpToFall / Falling logikához

        // Futás logika időküszöbbel: nem áll le azonnal, ha egy pillanatra leesik a sebesség
        bool shouldRunNow = grounded && speedX > runSpeedThreshold;

        if (shouldRunNow)
        {
            // azonnal fut, ha elég gyors
            isRunning = true;
            runStopTimer = 0f;
        }
        else
        {
            if (isRunning)
            {
                // már futott, lassul – csak akkor állítsd le, ha egy ideje lassú
                runStopTimer += Time.deltaTime;
                if (runStopTimer >= runStopDelay)
                {
                    isRunning = false;
                }
            }
            else
            {
                // már nem fut, maradjon így
                isRunning = false;
            }
        }

        animator.SetBool("IsRunning", isRunning);

        bool isFalling = !grounded && speedY < fallSpeedThreshold;
        animator.SetBool("IsFalling", isFalling);

        // Landing trigger: levegőből érkezünk földre
        if (!wasGroundedLastFrame && grounded)
        {
            animator.SetTrigger("Land");
        }

        wasGroundedLastFrame = grounded;
    }

    /// <summary>
    /// State váltásokhoz kötött triggereket kezeljük itt (pl. Jump).
    /// </summary>
    void HandleStateChanged(State prev, State current)
    {
        if (!animator) return;

        // Felugrás pillanata → "Jump" animáció (elrugaszkodás)
        if (current == State.Jump)
        {
            animator.SetTrigger("Jump");
        }

        // Ha nagyon akarod, ide tehetsz még extra kezelést,
        // pl. Fall state-be lépéskor külön trigger, de a fenti
        // VerticalSpeed alapján általában elég az Update-ben kezelt logika.
    }
}
