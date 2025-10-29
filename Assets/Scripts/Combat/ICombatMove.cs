using Unity.VisualScripting;
using UnityEngine;

public abstract class ICombat
{
    private float coolDown;
    private float activeTime;   //�t�s 
    private float windUpTime;   //�t�s el�tt
    private float recoveryTime; //�t�sutan
    private float damage;
    private float knockbackForce;
    private float knockbackDuration;
    private PlayerController controller;
    private Collider2D hitbox;
    private bool isAttacking;
    private float cooldown;
    private bool stateLock;
    private bool canEnterupted;
    private CharacterStateController CharacterStateController;

    public void TryAttack() 
    {
        if (CharacterStateController.CanAttack) 
        {
            isAttacking = true;
            PreformAttack();
        }
    }
    private void PreformAttack() 
    {
        
    }
    public abstract void Init() ;//inicializ�l�s

    public abstract void Execute();


}
