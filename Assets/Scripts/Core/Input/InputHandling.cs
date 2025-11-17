using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
/// <summary>
/// Beállítja a karakter adatait és inicializálja az input rendszert.
/// </summary>
/// <param name="data">A karakter tulajdonságait tartalmazó objektum.</param>
public class InputHandling : MonoBehaviour
{
    public PlayerController playerController;
    
    [SerializeField] PlayerInput playerInput;

    public float Movement;
    public event Action OnJumpPressed;
    public event Action OnJumpReleased;
    public event Action OnLightAttack;
    public event Action OnHeavyAttack;
    public event Action OnSpecialMove;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction lightAttackAction;
    InputAction heavyAttackAction;
    InputAction specialMoveAction;

    // HOLD STATE
    private bool isLightHeld;
    private float lightHoldStartTime;

    private bool isHeavyHeld;
    private float heavyHoldStartTime;

    private bool isSpecialHeld;
    private float specialHoldStartTime;

    public bool IsLightHeld => isLightHeld;
    public float LightHoldDuration => isLightHeld ? Time.time - lightHoldStartTime : 0f;

    public bool IsHeavyHeld => isHeavyHeld;
    public float HeavyHoldDuration => isHeavyHeld ? Time.time - heavyHoldStartTime : 0f;

    public bool IsSpecialHeld => isSpecialHeld;
    public float SpecialHoldDuration => isSpecialHeld ? Time.time - specialHoldStartTime : 0f;

    /// <summary>
    /// Inicializálja az input action-öket és hozzárendeli az eseményeket.
    /// Ezt egyszer kell meghívni, amikor a karakter létrejön vagy aktiválódik.
    /// </summary>
    public void Initialize()
    {

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        lightAttackAction = playerInput.actions["LightAttack"];
        heavyAttackAction = playerInput.actions["HeavyAttack"];
        specialMoveAction = playerInput.actions["SpecialMove"];

        // Bind callbacks


        jumpAction.performed += ctx => OnJumpPressed?.Invoke();
        jumpAction.canceled += ctx => OnJumpReleased?.Invoke();

        lightAttackAction.performed += ctx => OnLightAttack?.Invoke();
        heavyAttackAction.performed += ctx => OnHeavyAttack?.Invoke();
        specialMoveAction.performed += ctx => OnSpecialMove?.Invoke();

        playerInput.actions.Enable();
    }

    public void Tick()
    {
        Movement = moveAction.ReadValue<float>();

        UpdateHoldState(lightAttackAction, ref isLightHeld, ref lightHoldStartTime);
        UpdateHoldState(heavyAttackAction, ref isHeavyHeld, ref heavyHoldStartTime);
        UpdateHoldState(specialMoveAction, ref isSpecialHeld, ref specialHoldStartTime);
    }

    private void UpdateHoldState(InputAction action, ref bool isHeld, ref float holdStartTime)
    {
        if (action == null) return;

        bool pressed = action.IsPressed();
        if (pressed)
        {
            if (!isHeld)
            {
                isHeld = true;
                holdStartTime = Time.time;
            }
        }
        else
        {
            isHeld = false;
        }
    }

    /// <summary>
    /// Események eltávolítása, ha az objektum letiltódik (memory leak elkerülése).
    /// </summary>
    private void OnDisable()
    {
        if (playerInput == null) return;

        jumpAction.performed -= ctx => OnJumpPressed?.Invoke();
        jumpAction.canceled -= ctx => OnJumpReleased?.Invoke();

        lightAttackAction.performed -= ctx => OnLightAttack?.Invoke();
        heavyAttackAction.performed -= ctx => OnHeavyAttack?.Invoke();
        specialMoveAction.performed -= ctx => OnSpecialMove?.Invoke();
    }
}
