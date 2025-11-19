using JetBrains.Annotations;
using UnityEngine;
namespace Combat
{
    public enum CameraImpulseTriggerMode
    {
        OnEnter,
        OnLanding,
        OnExit
    }

    [CreateAssetMenu(menuName = "Combat/Steps/Camera Impulse (not on hit)")]
    public class CameraImpulseStep : AttackStep
    {
        [Header("Camera Impulse")]
        [Tooltip("Mennyire erõsen rázzuk a kamerát.")]
        public float magnitude = 1f;

        [Tooltip("Meddig tartson a rázkódás (sec).")]
        public float duration = 0.15f;

        [Header("Trigger mode")]
        public CameraImpulseTriggerMode triggerMode = CameraImpulseTriggerMode.OnEnter;

        private bool fired;

        public override void OnEnter(AttackRunner ctx)
        {
            if (triggerMode != CameraImpulseTriggerMode.OnEnter) return;

            if (fired) return;

            Fire();
        }

        public override void Tick(AttackRunner ctx, float localTime, float dt)
        {
            if (fired) return;

            // A Runner úgy hívja, hogy csak a saját idõsávjában fusson,
            // így nem kell külön startTime/endTime-et nézni.

            if (triggerMode == CameraImpulseTriggerMode.OnLanding)
            {
                if (ctx.LandedThisAttack)
                {
                    Fire();
                }
            }
        }

        public override void OnExit(AttackRunner ctx)
        {
            if (triggerMode == CameraImpulseTriggerMode.OnExit) Fire();
            fired = false;
        }

        private void Fire()
        {
            fired = true;
            CameraImpulseSystem.TriggerShake(magnitude, duration);
        }
    }
}


