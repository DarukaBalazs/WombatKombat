using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hurtbox : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] StockSystem stock;
    [SerializeField] KnockbackHandler kb;
    [SerializeField] PlayerStateManager state;
    [SerializeField] GameObject HitEffect;
    public AudioSource audioSource;
    public AudioClip HurtSound;
    private void Awake()
    {
         if (col == null) col = GetComponent<Collider2D>();
         col.isTrigger = true;
    }
    
    public void RecieveHit(Hitbox.HitInfo info, float percentgain, float knockbackForce, float hitstun )
    {
        if (info.attacker != null)
        {
            var runner = info.attacker.GetComponentInParent<AttackRunner>();
            if (runner != null)
            {
                runner.MarkHitConnected();
            }
        }
        if (!stock) stock = GetComponent<StockSystem>();
        bool crit = stock ? stock.RegisterHitAndRollCrit(percentgain) : false;
        if (!kb) kb = GetComponent<KnockbackHandler>();
        Vector2 dir = (transform.position - info.attacker.transform.position).normalized;
        Instantiate(HitEffect, transform.position, Quaternion.FromToRotation(transform.position, info.attacker.transform.position));
        float finalKnockback = knockbackForce;
        if (crit)
        {
            finalKnockback *= stock.GiveForceWithCrit();
            Debug.Log(stock.GiveForceWithCrit());
        }
        float finalHitstun = hitstun;

        if (state != null && state.HasSuperArmor)
        {
            finalKnockback *= 0.2f;
            finalHitstun *= 0.5f;
        }
        int rnd = Random.Range(1, 5);
        if (rnd==4)
        {
            audioSource.PlayOneShot(HurtSound);
        }
        kb.ApplyKnockback(dir, finalKnockback,crit,finalHitstun);

    }
}
