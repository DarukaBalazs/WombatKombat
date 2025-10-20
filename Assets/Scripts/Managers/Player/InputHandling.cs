using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Be�ll�tja a karakter adatait �s inicializ�lja az input rendszert.
/// </summary>
/// <param name="data">A karakter tulajdons�gait tartalmaz� objektum.</param>
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
    /// Inicializ�lja az input action-�ket �s hozz�rendeli az esem�nyeket.
    /// Ezt egyszer kell megh�vni, amikor a karakter l�trej�n vagy aktiv�l�dik.
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
    /// Esem�nyek elt�vol�t�sa, ha az objektum letilt�dik (memory leak elker�l�se).
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
