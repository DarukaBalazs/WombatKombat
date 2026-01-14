using UnityEngine;

public class PlatformSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] float radius = 3f; 
    [SerializeField] float angle = 30f;
    [SerializeField] float speed = 1f;      
    [SerializeField] Transform pivot;        

    private float t;

    void Update()
    {
        t += Time.deltaTime * speed;


        float currentAngle = Mathf.Sin(t) * angle; 

        // Szög radiánra alakítása
        float rad = currentAngle * Mathf.Deg2Rad;

        // Platform pozíció számítása köríven
        float x = pivot.position.x + Mathf.Sin(rad) * radius;
        float y = pivot.position.y - Mathf.Cos(rad) * radius;

        transform.position = new Vector3(x, y, transform.position.z);
    }
}