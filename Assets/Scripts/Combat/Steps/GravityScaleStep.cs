using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Steps/Gravity Scale")]
    public class GravityScaleStep : AttackStep
    {
        [SerializeField] private float scale;

        private float originalGravity;

        public override void OnEnter(AttackRunner runner)
        {
            var body = runner.Body;
            originalGravity = body.gravityScale;
            body.gravityScale = originalGravity * scale;
        }

        public override void OnExit(AttackRunner runner)
        {
            runner.Body.gravityScale = originalGravity;
        }
    }
}
