using UnityEngine;
using System.Collections.Generic;

public class HitboxGroup : MonoBehaviour
{
    [SerializeField] private Hitbox[] hitboxes; // ha üres, Awake-ben felderítjük
    private readonly Dictionary<string, Hitbox> map = new();

    private void Awake()
    {
        if (hitboxes == null || hitboxes.Length == 0)
            hitboxes = GetComponentsInChildren<Hitbox>(true);

        map.Clear();
        foreach (var hb in hitboxes)
            if (hb && !string.IsNullOrEmpty(hb.Id))
                map[hb.Id] = hb;
    }

    public Hitbox Get(string id) => map.TryGetValue(id, out var hb) ? hb : null;

    public IEnumerable<Hitbox> GetMany(IEnumerable<string> ids)
    {
        foreach (var id in ids)
            if (map.TryGetValue(id, out var hb)) yield return hb;
    }

    public void SetActiveOnly(IEnumerable<string> ids, bool active)
    {
        // elõbb minden OFF
        foreach (var hb in hitboxes) if (hb) hb.SetActive(false);
        // majd a kért(ek) ON
        foreach (var hb in GetMany(ids)) hb?.SetActive(active);
    }

    public void SetAll(bool active)
    {
        foreach (var hb in hitboxes) if (hb) hb.SetActive(active);
    }
}