using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Steps/Super Armor")]
    public class SuperArmorStep : AttackStep
    {
        public override void OnEnter(AttackRunner ctx)
        {
            if (ctx.State != null)
                ctx.State.EnableSuperArmor();
        }

        public override void OnExit(AttackRunner ctx)
        {
            if (ctx.State != null)
                ctx.State.DisableSuperArmor();
        }
    }
}
