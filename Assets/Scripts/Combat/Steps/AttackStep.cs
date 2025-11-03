using UnityEngine;
using Combat;

[CreateAssetMenu(fileName = "AttackStep", menuName = "Combat/Step")]
public abstract class AttackStep : ScriptableObject
{
    [Tooltip("Szerkesztési név, debughoz")]
    public string stepName;
 
    [Tooltip("Lokalizált idõ az ACITVE fázison belül (second)")]
    public float startTime;
    public float endTime;

    public virtual void Tick(AttackRunner ctx, float localTime, float dt) { }
    public virtual void OnEnter(AttackRunner ctx) { }
    public virtual void OnExit(AttackRunner ctx) { }
}
