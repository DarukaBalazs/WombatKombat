using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Steps/Movement Override")]
    public class MovementOverrideStep : AttackStep
    {
        [Header("When")]
        [Tooltip("Ha igaz, csak akkor írja felül a sebességet, ha a karakter a földön van.")]
        public bool onlyIfGrounded = true;
        public bool returnBase = false;

        [Header("Horizontal override")]
        [Tooltip("Felülírja az X sebességet (pl. 0 = teljes megállás).")]
        public bool overrideX = true;
        public float xVelocity = 0f;

        [Header("Vetical override")]
        public bool overrideY = false;
        public float yVelocity = 0f;

        private Vector2 originalVelocity;
        private bool applied;

        public override void OnEnter(AttackRunner ctx)
        {
            var rb = ctx.Body;
            if (rb == null) return;

            if (onlyIfGrounded && (ctx.State == null || !ctx.State.IsGrounded)) return;

            originalVelocity = rb.linearVelocity;

            Vector2 v = rb.linearVelocity;
            if (overrideX) { v.x  = xVelocity; }
            if (overrideY) { v.y = yVelocity; }

            rb.linearVelocity = v;
            applied = true;
        }

        public override void OnExit(AttackRunner ctx)
        {
            if (applied && returnBase) ctx.Body.linearVelocity = originalVelocity;
        }
    }
}
