using UnityEngine;

public class PlatformSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] float radius = 3f;      // lánc hossza (amennyire lelóg)
    [SerializeField] float angle = 30f;      // lengés szöge (bal-jobb)
    [SerializeField] float speed = 1f;       // lengés sebessége
    [SerializeField] Transform pivot;        // a felfüggesztés pontja

    private float t;

    void Update()
    {
        t += Time.deltaTime * speed;

        // Szög számítása szinuszos oda-vissza mozgással
        float currentAngle = Mathf.Sin(t) * angle;   // -angle -> +angle

        // Szög radiánra alakítása
        float rad = currentAngle * Mathf.Deg2Rad;

        // Platform pozíció számítása köríven
        float x = pivot.position.x + Mathf.Sin(rad) * radius;
        float y = pivot.position.y - Mathf.Cos(rad) * radius;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}