using UnityEngine;

public abstract class ICombat
{
    private float coolDown;
    private float activeTime;   //ütés 
    private float windUpTime;   //ütés elött
    private float recoveryTime; //ütésutan

    public abstract void Init() ;//inicializálás

    public abstract void Execute();


}
