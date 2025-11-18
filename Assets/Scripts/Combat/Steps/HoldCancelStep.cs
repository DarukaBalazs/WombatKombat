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

    [CreateAssetMenu(menuName ="Combat/Steps/HoldCancel")]
    public class HoldCancelStep : AttackStep
    {
        [Header("Hold beállítások")]
        public AttackButton button = AttackButton.Heavy;

        [Tooltip("Opcionális max hold idő (sec). 0 = nincs limit.")]
        public float maxHoldTime = 0f;

        private float elapsed;

        public override void OnEnter(AttackRunner ctx)
        {
            elapsed = 0f;
        }

        public override void Tick(AttackRunner ctx, float localTime, float dt)
        {
            elapsed += dt;

            bool held = IsButtonHeld(ctx);

            // ha már nem tartja, vagy túlléptük a maxHoldTime-et → teljes attack cancel
            if (!held || (maxHoldTime > 0f && elapsed >= maxHoldTime))
            {
                ctx.CancelCurrent();
            }
        }

        private bool IsButtonHeld(AttackRunner ctx)
        {
            var input = ctx.Input;
            switch (button)
            {
                case AttackButton.Light:
                    return input.IsLightHeld;
                case AttackButton.Heavy:
                    return input.IsHeavyHeld;
                case AttackButton.Special:
                    return input.IsSpecialHeld;
                default:
                    return false;
            }
        }
    }
}