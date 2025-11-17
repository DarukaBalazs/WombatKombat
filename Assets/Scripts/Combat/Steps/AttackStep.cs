using UnityEngine;

namespace Combat
{
    public enum AttackButton
    {
        None,
        Light,
        Heavy,
        Special
    }
    public abstract class AttackStep : ScriptableObject
    {
        [Header("Base")]
        [Tooltip("Szerkesztési név, debughoz")]
        public string stepName;

        [Tooltip("Idõ az ACTIVE fázison belül (sec)")]
        public float startTime;
        public float endTime;

        [Header("Hold Input")]
        public bool requiresHold = false;
        public AttackButton holdButton = AttackButton.None;
        public float minHoldTime = 0f;

        public virtual void Setup(GameObject go) { }
        public virtual void Tick(AttackRunner ctx, float localTime, float dt) { }
        public virtual void OnEnter(AttackRunner ctx) { }
        public virtual void OnExit(AttackRunner ctx) { }

        protected bool IsHoldSatisfied(AttackRunner ctx)
        {
            if (!requiresHold || holdButton == AttackButton.None)
                return true;

            var input = ctx.Input;

            switch (holdButton)
            {
                case AttackButton.Light:
                    return input.IsLightHeld && input.LightHoldDuration >= minHoldTime;

                case AttackButton.Heavy:
                    return input.IsHeavyHeld && input.HeavyHoldDuration >= minHoldTime;

                case AttackButton.Special:
                    return input.IsSpecialHeld && input.SpecialHoldDuration >= minHoldTime;
            }

            return true;
        }
    }
}

