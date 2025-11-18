using UnityEngine;
using UnityEngine.InputSystem.Utilities;
namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Steps/Camera Impulse")]
    public class CameraImpulseStep : AttackStep
    {
        public float magnitude;

        public float duration;
        public override void OnEnter(AttackRunner ctx)
        {
            CameraImpulseSystem.TriggerShake(magnitude, duration);
        }
    }
}


