using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Steps/Gravity Scale")]
    public class GravityScaleStep : AttackStep
    {
        [Header("Paraméterek")]
        public float scale;
        private float original;

        public override void OnEnter(AttackRunner ctx)
        {
            original = ctx.Body.gravityScale;

            ctx.Body.gravityScale = scale;
        }

        public override void OnExit(AttackRunner ctx)
        {
            ctx.Body.gravityScale = original;
        }
    }
}
