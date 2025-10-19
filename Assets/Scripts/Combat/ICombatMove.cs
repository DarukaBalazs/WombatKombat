using UnityEngine;

public abstract class ICombat
{
    private float coolDown;
    private float activeTime;   //�t�s 
    private float windUpTime;   //�t�s el�tt
    private float recoveryTime; //�t�sutan

    public abstract void Init() ;//inicializ�l�s

    public abstract void Execute();


}
