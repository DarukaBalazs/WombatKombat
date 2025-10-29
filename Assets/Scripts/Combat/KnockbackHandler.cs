using UnityEngine;
using System.Collections;
public class KnockbackHandler : MonoBehaviour
{
    private CharacterStateController controller;
    private Rigidbody2D rb;

    public void ApplyKnockback(Vector2 direction, float force,float duration) 
    {
        StartCoroutine(CorutinKonockback(direction, force, duration));
    }
    private IEnumerator CorutinKonockback(Vector2 direction, float force, float duration)
    {
        controller.SetKnockBack(true);
        rb.AddForceX(direction.x*force);
        rb.AddForceY(direction.y*force);
        yield return new WaitForSecondsRealtime(duration);
        controller.SetKnockBack(false);

    }

}
