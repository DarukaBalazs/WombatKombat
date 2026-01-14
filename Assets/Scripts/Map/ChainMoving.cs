using UnityEngine;

public class ChainVisual : MonoBehaviour
{
    public Transform pivot;        
    public Transform platform;     
    public float chainWidth = 0.1f;

    private SpriteRenderer sr;
    private float spriteWorldHalfHeight;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

     
        spriteWorldHalfHeight = sr.bounds.size.y * 0.5f;
    }

    void LateUpdate()
    {
        if (!pivot || !platform) return;

        Vector3 top = pivot.position;
        Vector3 bottom = platform.position - new Vector3(0, 6.5f, 0);

        // két pont középpontja
        Vector3 mid = (top + bottom) * 0.5f;

        // irány a pivot -> platform
        Vector3 dir = bottom - top;
        float dist = dir.magnitude;
        if (dist < 0.0001f) return;

        // irány beállítása
        transform.up = dir.normalized;

        // pozíció = középpont
        transform.position = mid;

        // skála: úgy állítjuk, hogy a sprite vége pont érjen a pontokhoz
        float neededHalfHeight = dist * 0.5f;
        float scaleY = neededHalfHeight / spriteWorldHalfHeight;

        transform.localScale = new Vector3(chainWidth, scaleY, 1f);
    }
}