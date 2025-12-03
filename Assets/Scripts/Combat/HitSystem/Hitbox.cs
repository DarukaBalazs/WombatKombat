using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] string id;
    private GameObject owner;

    public string Id => id;
    public struct HitInfo
    {
        public GameObject attacker;
        public Vector2 hitPoint;
        public Vector2 hitNormal;
    }

    public event Action<Hurtbox,HitInfo> OnHit;

    private void Awake()
    {
        if (!col) col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public void SetOwner(GameObject o) => owner = o;

    public void SetActive(bool active)
    {
        col.enabled = active;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!col.enabled) return;

        var hurt = collision.GetComponent<Hurtbox>();

        if (!hurt) return;

        if(hurt.gameObject == owner ) return;

        var info = new HitInfo
        {
            attacker = owner,
            hitPoint = collision.ClosestPoint(transform.position),
            hitNormal = (collision.transform.position - transform.position).normalized
        };

        OnHit?.Invoke(hurt, info);
    }
}
