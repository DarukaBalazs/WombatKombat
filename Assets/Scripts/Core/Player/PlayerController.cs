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
    [SerializeField] PlayerStateManager state;
    [SerializeField] BaseMovement movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AttackRunner attackRunner;
    [SerializeField] AttackAsset lightAttack;
    [SerializeField] AttackAsset heavyAttack;
    [SerializeField] AttackAsset specialAttack;

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

    private void Awake()
    {
        input.OnJumpPressed += movement.HandleJump;
        input.OnJumpReleased += movement.CancelJump;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        input.Tick();
        state.Tick(dt);
        movement.Tick(dt);
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        state.FixedTick(dt);
        movement.HandleMove(input.Movement);
        movement.FixedTick(dt);
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
        ApplyLoadout(data.loadout);
    }

    public void ApplyLoadout(CharacterLoadout lo)
    {
        lightAttack = lo?.lightAttack ?? null;
        heavyAttack = lo?.heavyAttack ?? null;
        specialAttack = lo?.specialAttack ?? null;
        input.OnLightAttack += () => { attackRunner.TryStart(lightAttack); animator.SetTrigger("LightPressed"); };
        input.OnHeavyAttack += () => attackRunner.TryStart(heavyAttack);
        input.OnSpecialMove += () => attackRunner.TryStart(specialAttack);
    }

}
