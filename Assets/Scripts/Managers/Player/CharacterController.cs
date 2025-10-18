using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] InputHandling input;
    [SerializeField] BaseMovement movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Character Data")]
    [SerializeField] CharacterData data;

    [Header("Modules")]
    //[SerializeField] List<SpecialMovement> specialMovements;
    //[SerializeField] List<AttackAction> attacks;

    public Rigidbody2D Rb => rb;
    public Animator Animator => animator;
    public CharacterData Data => data;

    private void Update()
    {
        movement.HandleMove(input.MoveInput.x);

        if (input.JumpPressed)
        {
            Debug.Log("pressed");
            movement.HandleJump();
        }

        if (input.JumpReleased)
        {
            Debug.Log("Released");
            movement.CancelJump();
        }

    }

    public void ApplyCharacterData(CharacterData data)
    {
        movement.ApplyCharacterData(data);
        this.data = data;
        input.Initialize();
    }

}
