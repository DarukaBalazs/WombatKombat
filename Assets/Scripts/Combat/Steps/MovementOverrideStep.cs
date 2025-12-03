using UnityEngine;

namespace Combat
{
    public enum GroundFilter
    {
        Any,            // mindegy, földön vagy levegőben
        OnlyGrounded,   // csak ha földön van
        OnlyAir         // csak ha levegőben van
    }

    public enum DirectionFilter
    {
        Any,        // mindegy, merre tartja a botot
        Up,         // felfelé input
        Down,       // lefelé input
        Neutral     // nincs fel/le input
    }

    [CreateAssetMenu(menuName = "Combat/Steps/Movement Override")]
    public class MovementOverrideStep : AttackStep
    {
        [Header("When")]
        [Tooltip("Föld/levego szűrés.")]
        public GroundFilter groundFilter = GroundFilter.Any;

        [Tooltip("Fel/le/semleges input szűrés.")]
        public DirectionFilter directionFilter = DirectionFilter.Any;

        public bool returnBase = false;

        [Header("Horizontal override")]
        [Tooltip("Felülírja az X sebességet (pl. 0 = teljes megállás).")]
        public bool overrideX = true;
        public float xVelocity = 0f;

        [Header("Vetical override")]
        public bool overrideY = false;
        public float yVelocity = 0f;

        private Vector2 originalVelocity;
        private bool applied;

        public override void OnEnter(AttackRunner ctx)
        {
            var rb = ctx.Body;
            if (rb == null) return;

            if (!PassesFilters(ctx)) return;

            originalVelocity = rb.linearVelocity;
            
            Vector2 v = rb.linearVelocity;
            float facingSign = Mathf.Sign(rb.transform.localScale.x);
            if (facingSign == 0f) facingSign = 1f;

            if (overrideX) { v.x  = xVelocity * facingSign; }
            if (overrideY) { v.y = yVelocity; }

            rb.linearVelocity = v;
            applied = true;
        }

        public override void OnExit(AttackRunner ctx)
        {
            if (applied && returnBase) ctx.Body.linearVelocity = originalVelocity;
        }

        private bool PassesFilters(AttackRunner ctx)
        {
            var state = ctx.State;

            // Ground / Air szűrés
            if (groundFilter == GroundFilter.OnlyGrounded)
            {
                if (state == null || !state.IsGrounded)
                    return false;
            }
            else if (groundFilter == GroundFilter.OnlyAir)
            {
                if (state == null || state.IsGrounded)
                    return false;
            }

            // Direction szűrés (fel/le/semleges input)
            if (directionFilter != DirectionFilter.Any)
            {
                var input = ctx.Input;
                if (input == null)
                    return false;

                float vertical = input.Vertical; 

                const float UP_THRESHOLD = 0.5f;
                const float DOWN_THRESHOLD = -0.5f;
                const float NEUTRAL_DEADZONE = 0.2f;

                switch (directionFilter)
                {
                    case DirectionFilter.Up:
                        if (vertical < UP_THRESHOLD) return false;
                        break;

                    case DirectionFilter.Down:
                        if (vertical > DOWN_THRESHOLD) return false;
                        break;

                    case DirectionFilter.Neutral:
                        if (Mathf.Abs(vertical) > NEUTRAL_DEADZONE) return false;
                        break;
                }
            }

            return true;
        }
    }
}
