using UnityEngine;
using UnityEngine.InputSystem;
public class InputHandling : MonoBehaviour
{
    public CharacterController characterController;

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

    private void LateUpdate()
    {
        JumpPressed = false;
        JumpReleased = false;
        LightAttackPressed = false;
        HeavyAttackPressed = false;
        SpecialMovePressed = false;
    }
}
