using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] StockSystem stock;
    [SerializeField] KnockbackHandler kb;

    private void Awake()
    {
         if (col == null) col = GetComponent<Collider2D>();
         col.isTrigger = true;
    }

    public void RecieveHit(Hitbox.HitInfo info, float percentgain, float knockbackForce, float hitstun )
    {
        if (!stock) stock = GetComponent<StockSystem>();
        bool crit = stock ? stock.RegisterHitAndRollCrit(percentgain) : false;
        if (!kb) kb = GetComponent<KnockbackHandler>();
        Vector2 dir = (transform.position - info.attacker.transform.position).normalized;
        kb.ApplyKnockback(dir, knockbackForce,hitstun);

    }
}
