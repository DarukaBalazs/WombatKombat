using UnityEngine;


namespace Combat 
{
    [CreateAssetMenu(menuName = "Combat/Steps/Invulnerability")]
    public class InvulnerabilityStep : AttackStep
    {
        [Header("Paraméterek")]
        public float duration;

        public override void OnEnter(AttackRunner ctx)
        {
            ctx.State.EnterInvulnerable(duration);
        }
    }
}
