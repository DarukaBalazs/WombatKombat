using UnityEngine;

public class CharacterStateController : MonoBehaviour
{
    private bool isAttacking = false;
    private bool isKnockBack = false;
    private bool isStunned=false;

    public bool CanMove=>!isAttacking && !isKnockBack && !isStunned;
    public bool CanAttack => CanMove;

    public void SetAttack(bool attack) 
    { 
        isAttacking = attack;
    }
    public void SetKnockBack(bool knockBack) 
    {
        isKnockBack=knockBack;
    }
    public void SetStunned(bool Stunned) 
    {
        isStunned = Stunned;
    
    }
}
