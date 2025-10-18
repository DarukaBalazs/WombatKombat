using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Be�ll�tja a karakter adatait �s inicializ�lja az input rendszert.
/// </summary>
/// <param name="data">A karakter tulajdons�gait tartalmaz� objektum.</param>
public class InputHandling : MonoBehaviour
{
    public PlayerController playerController;

    [Header("Current Input States")]
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool LightAttackPressed { get; private set; }
    public bool HeavyAttackPressed { get; private set; }
    public bool SpecialMovePressed { get; private set; }
    
    [SerializeField] PlayerInput playerInput;

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
        moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => MoveInput = Vector2.zero;

        jumpAction.performed += ctx => JumpPressed = true;
        jumpAction.canceled += ctx => JumpReleased = true;

        lightAttackAction.performed += ctx => LightAttackPressed = true;
        heavyAttackAction.performed += ctx => HeavyAttackPressed = true;
        specialMoveAction.performed += ctx => SpecialMovePressed = true;

        playerInput.actions.Enable();
    }

    /// <summary>
    /// Esem�nyek elt�vol�t�sa, ha az objektum letilt�dik (memory leak elker�l�se).
    /// </summary>
    private void OnDisable()
    {
        if (playerInput == null) return;

        moveAction.performed -= ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled -= ctx => MoveInput = Vector2.zero;

        jumpAction.performed -= ctx => JumpPressed = true;
        jumpAction.canceled -= ctx => JumpReleased = true; 

        lightAttackAction.performed -= ctx => LightAttackPressed = true;
        heavyAttackAction.performed -= ctx => HeavyAttackPressed = true;
        specialMoveAction.performed -= ctx => SpecialMovePressed = true;
    }

    /// <summary>
    /// Az input bool �rt�kek vissza�ll�t�sa minden frame v�g�n.
    /// Ezzel biztos�tjuk, hogy egy gombnyom�s csak egy frame-re �rv�nyes.
    /// </summary>
    private void LateUpdate()
    {
        JumpPressed = false;
        JumpReleased = false;
        LightAttackPressed = false;
        HeavyAttackPressed = false;
        SpecialMovePressed = false;
    }
}
