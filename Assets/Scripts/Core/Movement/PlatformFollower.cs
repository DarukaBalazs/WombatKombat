using UnityEngine;

public class PlatformFollower : MonoBehaviour
{
    Transform platform;
    Vector3 lastPos;
    bool onPlatform;

    void FixedUpdate()
    {
        if (!onPlatform || platform == null)  
            return;

        Vector3 delta = platform.position - lastPos;
        transform.position += delta;
        lastPos = platform.position;
    }

    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("MovingPlatform"))
        {
            platform = c.transform;
            lastPos = platform.position;
            onPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("MovingPlatform"))
        {
            onPlatform = false;
            platform = null;
        }
    }
}
