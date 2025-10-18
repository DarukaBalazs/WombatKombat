using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Beállítja a karakter adatait és inicializálja az input rendszert.
/// </summary>
/// <param name="data">A karakter tulajdonságait tartalmazó objektum.</param>
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
    /// Események eltávolítása, ha az objektum letiltódik (memory leak elkerülése).
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
    /// Az input bool értékek visszaállítása minden frame végén.
    /// Ezzel biztosítjuk, hogy egy gombnyomás csak egy frame-re érvényes.
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
