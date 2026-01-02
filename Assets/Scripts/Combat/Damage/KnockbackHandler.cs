using UnityEngine;

/// <summary>
/// Egyszerû, konfigurálható knockback-kezelõ 2D-hez (Unity 6).
/// - Azonnali impulzust ad a Rigidbody2D-nek (linearVelocity).
/// - Beállítható kritikus szorzó, komponens-szorzók, max sebesség clamp.
/// - Hitstun-t kér a PlayerStateManager-tõl.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerStateManager state;
    [SerializeField] private StockSystem stockSystem;

    [Header("Force setup")]
    [Tooltip("Kritikus találat erõszorzója.")]
    [SerializeField] private float critMultiplier = 3f;

    [Tooltip("Vízszintes komponens szorzó.")]
    [SerializeField] private float horizontalMultiplier = 1f;

    [Tooltip("Függõleges komponens szorzó.")]
    [SerializeField] private float verticalMultiplier = 1f;

    [Tooltip("Impulzus hozzáadása a jelenlegi sebességhez (true) vagy teljes csere (false).")]
    [SerializeField] private bool additive = true;

    [Tooltip("Maximális engedett sebesség knockback után.")]
    [SerializeField] private float maxSpeed = 150f;

    [Header("Hitstun")]
    [Tooltip("Minimum hitstun idõ (s).")]
    [SerializeField] private float minStunSeconds = 0.1f;

    [Tooltip("Opcionális extra drag a stun ideje alatt. 0 = nincs.")]
    [SerializeField] private float stunLinearDrag = 0f;

    private float _prevDrag;
    private bool _dragApplied;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        state = GetComponent<PlayerStateManager>();
        maxSpeed = 30f;
        critMultiplier = 3f;
        horizontalMultiplier = 1f;
        verticalMultiplier = 1f;
        additive = true;
        minStunSeconds = 0.1f;
        stunLinearDrag = 0f;
    }

    /// <summary>
    /// Knockback alkalmazása. A dir normálása automatikus.
    /// </summary>
    /// <param name="dir">Lökés iránya (támadótól kifelé).</param>
    /// <param name="baseForce">Alaperõ.</param>
    /// <param name="isCrit">Kritikus találat?</param>
    /// <param name="stunSeconds">Hitstun idõ (s).</param>
    public void ApplyKnockback(Vector2 dir, float baseForce, bool isCrit, float stunSeconds)
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!state) state = GetComponent<PlayerStateManager>();
        if (state != null && state.IsDead) return;

        // Irány & erõ
        Vector2 n = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.up;
        float mult = isCrit ? critMultiplier : 1f;
        Vector2 force = new Vector2(n.x * horizontalMultiplier, n.y * verticalMultiplier) * (baseForce * mult);

        // Sebesség beállítás
        Vector2 v = rb.linearVelocity;
        if (additive) v += force;
        else v = force;

        // Clamp
        if (v.sqrMagnitude > maxSpeed * maxSpeed)
            v = v.normalized * maxSpeed;

        rb.linearVelocity = v;

        // Stun kérés + opcionális drag
        float stun = Mathf.Max(stunSeconds, minStunSeconds);
        if (stun > 0f)
        {
            if (stunLinearDrag > 0f && !_dragApplied)
            {
                _prevDrag = rb.linearDamping; // Unity 6: linearDamping a 2D drag megfelelõje
                rb.linearDamping = stunLinearDrag;
                _dragApplied = true;
                // Drag visszaállítása stun lejártakor
                StartCoroutine(RestoreDragAfter(stun));
            }

            state?.EnterHitstun(stun);
        }
    }

    /// <summary>
    /// Egyszerûsített overload: nem kritikus.
    /// </summary>
    public void ApplyKnockback(Vector2 dir, float baseForce, float stunSeconds)
        => ApplyKnockback(dir, baseForce, false, stunSeconds);

  

    private System.Collections.IEnumerator RestoreDragAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (rb)
        {
            rb.linearDamping = _prevDrag;
        }
        _dragApplied = false;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
            rb.linearVelocity.x.ToString());
    }
#endif
}