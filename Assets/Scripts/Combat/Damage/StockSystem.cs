using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class StockSystem : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("References")]
    [SerializeField] PlayerStateManager state;
    [SerializeField] Rigidbody2D rb;
    [Header("Sound")]
    public AudioClip Victory;
    public AudioClip Sound;
    [Header("Stocks & Damage")]
    [SerializeField] float stocks = 5;
    [SerializeField, Range(0, 999)] float damagePercent = 0f;

    [Header("Crit knockback")]
    [SerializeField, Range(0, 100)] float maxCritChance = 100f;
    [SerializeField] AnimationCurve percentToCritchance = AnimationCurve.Linear(0, 0, 150, 100);
    [SerializeField] float onHitPercentGain = 8f;


    public float Stocks => stocks;
    public float DamagePercent => damagePercent;

    public bool RegisterHitAndRollCrit(float? percentGainOverride = null)
    {
        float gain = percentGainOverride ?? onHitPercentGain;
        damagePercent = Mathf.Clamp(damagePercent + gain, 0f, 999f);
        return RollCrit();
    }

    public bool RollCrit()
    {
        float chance = Mathf.Clamp(percentToCritchance.Evaluate(damagePercent),0f,maxCritChance);
        return UnityEngine.Random.value < (chance / 100f);
    }

    public void LoseStockAndRespawn(Vector3 respawnPos)
    {
        audioSource.PlayOneShot(Sound);

        stocks = Mathf.Max(0, stocks - 1f);


        rb.linearVelocity = Vector2.zero;
        transform.position = respawnPos;
        ResetPercent();
        state?.ForceTransition(State.Idle);

        if (stocks <= 0)
        {
            state?.ForceKill();
            audioSource.PlayOneShot(Victory);
        }

    }

    public float GiveForceWithCrit()
    {
        return Mathf.Clamp(damagePercent/200, 1f, 4f);
    }

    public void ResetPercent()
    {
        damagePercent = 0f;
    }
}
