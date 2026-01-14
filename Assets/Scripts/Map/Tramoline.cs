using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] float bounceForce = 18f;  

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // csak Rigidbody2D-s dolgokra reagálunk
        var rb = collision.rigidbody;
        if (rb == null) return;

        if (collision.relativeVelocity.y <= 0f)
        {

            Vector2 v = rb.linearVelocity;
            v.y = bounceForce;            
            rb.linearVelocity = v;
        }
    }
}