using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Steps/Hitbox Window")]
    public class HitboxWindowStep : AttackStep
    {
        [Header("Hitboxes to activate")]
        public string[] hitboxIds;

        [Header("Combat értékek")]
        public float percentGain = 10f;
        public float knockbackForce = 5f;
        public float hitstun = 0.2f;


        [Header("Camera shake")]
        public bool shakeCamera = false;
        public float shakeMagnitude = 1.0f;
        public float shakeDuration = 1.0f;

        [Tooltip("Találatkor ennyi ideig fagyjon be a játék (sec). 0 = nincs hitstop.")]
        public float hitstopDuration = 0.05f;

        private readonly List<Hitbox> activeHitboxes = new();

        public override void OnEnter(AttackRunner ctx)
        {
            activeHitboxes.Clear();

            foreach (var hb in ctx.Hitboxes.GetMany(hitboxIds))
            {
                if (!hb) continue;

                hb.SetOwner(ctx.gameObject);
                hb.OnHit += HandleHit;
                hb.SetActive(true);

                activeHitboxes.Add(hb);
            }
        }

        public override void OnExit(AttackRunner ctx)
        {
            foreach (var hb in activeHitboxes)
            {
                if (!hb) continue;

                hb.OnHit -= HandleHit;
                hb.SetActive(false);
            }

            activeHitboxes.Clear();
        }

        private void HandleHit(Hurtbox hurt, Hitbox.HitInfo info)
        {
            if (!hurt) return;

            hurt.RecieveHit(info, percentGain, knockbackForce, hitstun);
            if (hitstopDuration > 0f && HitstopManager.Instance != null)
            {
                HitstopManager.Instance.RequestHitstop(hitstopDuration);
            }
            if (shakeCamera)
            {
                if (shakeDuration > 0f)
                {
                    CameraImpulseSystem.TriggerShake(shakeMagnitude, shakeDuration);
                }
            }


        }
    }
}
