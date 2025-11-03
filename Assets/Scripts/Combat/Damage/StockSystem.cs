using UnityEngine;
using System;

public class StockSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerStateManager state;
    [SerializeField] Rigidbody2D rb;

    [Header("Stocks & Damage")]
    [SerializeField] int stocks = 5;
    [SerializeField, Range(0, 999)] float damagePercent = 0f;

    [Header("Crit knockback")]
    [SerializeField, Range(0, 100)] float maxCritChance = 100f;
    [SerializeField] AnimationCurve percentToCritchance = AnimationCurve.Linear(0, 0, 150, 60);
    [SerializeField] float onHitPercentGain = 8f;

    public event Action OnStockLost;
    public event Action<int> OnStockChanged;
    public event Action<float> OnPercentChanged;
    public event Action OnRespawn;

    public int Stocks => stocks;
    public float DamagePercent => damagePercent;

    public bool RegisterHitAndRollCrit(float? percentGainOverride = null)
    {
        float gain = percentGainOverride ?? onHitPercentGain;
        damagePercent = Mathf.Clamp(damagePercent + gain, 0f, 999f);
        OnPercentChanged?.Invoke(damagePercent);
        return RollCrit();
    }

    public bool RollCrit()
    {
        float chance = Mathf.Clamp(percentToCritchance.Evaluate(damagePercent),0f,maxCritChance);
        return UnityEngine.Random.value < (chance / 100f);
    }

    public void LoseStockAndRespawn(Vector3 respawnPos)
    {
        stocks = Mathf.Max(0, stocks - 1);
        OnStockLost?.Invoke();
        OnStockChanged?.Invoke(stocks);

        if (stocks <= 0)
        {
            state?.ForceKill();

            rb.linearVelocity = Vector2.zero;
            transform.position = respawnPos;
            ResetPercent();
            state?.ForceTransition(State.Idle);
            OnRespawn?.Invoke();
        }
    }

    public void ResetPercent()
    {
        damagePercent = 0f;
        OnPercentChanged?.Invoke(damagePercent);
    }
}
