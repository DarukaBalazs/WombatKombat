using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] float bounceForce = 18f;   // mennyire dobjon fel

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // csak Rigidbody2D-s dolgokra reagálunk
        var rb = collision.rigidbody;
        if (rb == null) return;

        // Ha fentről érkezik (a relatív sebesség lefelé mutat)
        if (collision.relativeVelocity.y <= 0f)
        {
            // opcionális: ide tehetsz Debug.Log-ot, hogy lásd, hogy fut-e
            // Debug.Log("TRAMPOLINE HIT: " + collision.gameObject.name);

            Vector2 v = rb.linearVelocity;
            v.y = bounceForce;            // állítsuk be a felfelé irányt
            rb.linearVelocity = v;
        }
    }
}