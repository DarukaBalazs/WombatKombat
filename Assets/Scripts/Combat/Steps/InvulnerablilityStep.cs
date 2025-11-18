using UnityEngine;


namespace Combat 
{
    [CreateAssetMenu(menuName = "Combat/Steps/Invulnerability")]
    public class InvulnerabilityStep : AttackStep
    {
        [SerializeField] float seconds;
        public override void OnEnter(AttackRunner ctx)
        {
            ctx.State.EnterInvulnerable(seconds);
        }
    }
}
