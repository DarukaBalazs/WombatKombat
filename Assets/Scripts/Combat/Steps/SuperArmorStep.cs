using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Steps/Super Armor")]
    public class SuperArmorStep : AttackStep
    {
        public override void OnEnter(AttackRunner ctx)
        {
            ctx.State.EnableSuperArmor();
        }

        public override void OnExit(AttackRunner ctx)
        {
            ctx.State.DisableSuperArmor();
        }
    }
}
