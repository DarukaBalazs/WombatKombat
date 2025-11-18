using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Combat/Steps/Directional Hitbox Window")]
    public class DirectionalHitboxWindowStep : AttackStep
    {
        [Header("Hitbox ID-k")]
        [Tooltip("Neutral / oldalra irány (alap támadás)")]
        public string[] forwardHitboxIds;

        [Tooltip("Lefelé irány (down attack)")]
        public string[] downHitboxIds;

        [Header("Combat értékek")]
        public float percentGain = 10f;

        [Tooltip("Knockback forward/neutral támadásnál")]
        public float knockbackForward = 5f;

        [Tooltip("Knockback down támadásnál")]
        public float knockbackDown = 5f;

        public float hitstun = 0.2f;

        [Header("Input küszöb")]
        [Tooltip("Ekkora negatív vertical érték alatt számít 'lefelé' támadásnak")]
        public float downThreshold = -0.5f;

        private readonly List<Hitbox> activeHitboxes = new();
        private float currentKnockback;

        public override void OnEnter(AttackRunner ctx)
        {
            activeHitboxes.Clear();

            var group = ctx.Hitboxes;
            if (!group) return;

            // Döntés az aktuális input alapján
            float v = ctx.Input.Vertical;

            string[] chosenIds;
            if (v <= downThreshold && downHitboxIds != null && downHitboxIds.Length > 0)
            {
                chosenIds = downHitboxIds;
                currentKnockback = knockbackDown;
            }
            else
            {
                chosenIds = forwardHitboxIds;
                currentKnockback = knockbackForward;
            }

            if (chosenIds == null || chosenIds.Length == 0)
                return;

            // Hitboxok aktiválása, owner beállítása, event feliratkozás
            foreach (var hb in group.GetMany(chosenIds))
            {
                if (!hb) continue;
                Debug.Log($"{nameof(hb)} fired");

                hb.SetOwner(ctx.gameObject);
                hb.OnHit += HandleHit;
                hb.SetActive(true);
                activeHitboxes.Add(hb);
            }
        }

        public override void OnExit(AttackRunner ctx)
        {
            // Leiratkozás + kikapcsolás
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

            // A választott iránynak megfelelõ knockbacket használjuk
            hurt.RecieveHit(info, percentGain, currentKnockback, hitstun);
        }
    }
}
