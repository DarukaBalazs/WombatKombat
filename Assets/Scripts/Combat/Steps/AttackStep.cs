using UnityEngine;

namespace Combat
{
    public abstract class AttackStep : ScriptableObject
    {
        [Header("Base")]

        [Tooltip("ACTIVE fázison belüli kezdet (sec)")]
        public float startTime;

        [Tooltip("ACTIVE fázison belüli vég (sec)")]
        public float endTime;

        public virtual void Setup(GameObject owner) { }
        public virtual void OnEnter(AttackRunner ctx) { }
        public virtual void Tick(AttackRunner ctx, float localTime, float dt) { }
        public virtual void OnExit(AttackRunner ctx) { }
    }
}