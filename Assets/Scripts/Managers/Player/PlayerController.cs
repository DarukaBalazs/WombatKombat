using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A karakter f� vez�rl�je, amely �sszefogja az inputkezel�st, a mozg�st �s az anim�ci�t.
/// Ez az oszt�ly kapcsolja �ssze a j�t�kos bevitel�t a mozg�slogik�val.
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
    /// A karakter aktu�lis adat�t (sebess�g, ugr�er� stb.) tartalmaz� objektum.
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
    /// Be�ll�tja a karakter adatait �s inicializ�lja az input rendszert.
    /// </summary>
    /// <param name="data">A karakter tulajdons�gait tartalmaz� objektum.</param>
    public void ApplyCharacterData(CharacterData data)
    {
        movement.ApplyCharacterData(data);
        this.data = data;
        input.Initialize();
    }

}
