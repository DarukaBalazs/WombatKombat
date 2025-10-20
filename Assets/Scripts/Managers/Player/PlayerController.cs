using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A karakter fõ vezérlõje, amely összefogja az inputkezelést, a mozgást és az animációt.
/// Ez az osztály kapcsolja össze a játékos bevitelét a mozgáslogikával.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] InputHandling input;
    [SerializeField] BaseMovement movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Character Data")]
    /// <summary>
    /// A karakter aktuális adatát (sebesség, ugróerõ stb.) tartalmazó objektum.
    /// </summary>
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
            movement.HandleJump();
        }

        if (input.JumpReleased)
        {
            movement.CancelJump();
        }

    }

    /// <summary>
    /// Beállítja a karakter adatait és inicializálja az input rendszert.
    /// </summary>
    /// <param name="data">A karakter tulajdonságait tartalmazó objektum.</param>
    public void ApplyCharacterData(CharacterData data)
    {
        movement.ApplyCharacterData(data);
        this.data = data;
        input.Initialize();
    }

}
