using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Beállítja a karakter adatait és inicializálja az input rendszert.
/// </summary>
/// <param name="data">A karakter tulajdonságait tartalmazó objektum.</param>
public class InputHandling : MonoBehaviour
{
    public PlayerController playerController;
    
    [SerializeField] PlayerInput playerInput;

    public event Action<Vector2> OnMove;
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
        moveAction.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => OnMove?.Invoke(Vector2.zero);

        jumpAction.performed += ctx => OnJumpPressed?.Invoke();
        jumpAction.canceled += ctx => OnJumpReleased?.Invoke();

        lightAttackAction.performed += ctx => OnLightAttack?.Invoke();
        heavyAttackAction.performed += ctx => OnHeavyAttack?.Invoke();
        specialMoveAction.performed += ctx => OnSpecialMove?.Invoke();

        playerInput.actions.Enable();
    }



    /// <summary>
    /// Események eltávolítása, ha az objektum letiltódik (memory leak elkerülése).
    /// </summary>
    private void OnDisable()
    {
        if (playerInput == null) return;
        moveAction.performed -= ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled -= ctx => OnMove?.Invoke(Vector2.zero);

        jumpAction.performed -= ctx => OnJumpPressed?.Invoke();
        jumpAction.canceled -= ctx => OnJumpReleased?.Invoke();

        lightAttackAction.performed -= ctx => OnLightAttack?.Invoke();
        heavyAttackAction.performed -= ctx => OnHeavyAttack?.Invoke();
        specialMoveAction.performed -= ctx => OnSpecialMove?.Invoke();
    }
}
