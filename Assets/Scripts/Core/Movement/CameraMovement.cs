using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Camera Settings")]
    [SerializeField] private float smoothTime = 0.3f;

    [Tooltip("Kamera legközelebbi Z távolság")]
    [SerializeField] private float minZoom = -8f;

    [Tooltip("Kamera legtávolabbi Z távolság")]
    [SerializeField] private float maxZoom = -16f;

    [SerializeField] private float zoomLimiter = 20f;

    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    [Header("Player Follow Bounds")]
    [SerializeField] private Vector2 playerMinBounds;
    [SerializeField] private Vector2 playerMaxBounds;

    Vector3 velocity;
    Camera cam;

    // SHAKE
    Vector3 shakeOffset;
    Coroutine shakeRoutine;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        CameraImpulseSystem.OnCameraShake += StartCameraShake;
    }

    private void OnDisable()
    {
        CameraImpulseSystem.OnCameraShake -= StartCameraShake;
        shakeOffset = Vector3.zero;
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 targetPos = centerPoint;

        targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);
        targetPos.z = transform.position.z;

        Vector3 basePos = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        transform.position = basePos + shakeOffset;
    }

    void Zoom()
    {
        bool p1Inside = IsInsidePlayerBounds(player1);
        bool p2Inside = IsInsidePlayerBounds(player2);

        if (!p1Inside || !p2Inside)
            return;

        float distance = Vector3.Distance(player1.position, player2.position);
        float t = Mathf.Clamp01(distance / zoomLimiter);

        float targetZ = Mathf.Lerp(minZoom, maxZoom, t);

        Vector3 pos = transform.position;
        pos.z = Mathf.Lerp(pos.z, targetZ, Time.deltaTime * 5f);
        transform.position = pos;
    }

    Vector3 GetCenterPoint()
    {
        bool p1Inside = IsInsidePlayerBounds(player1);
        bool p2Inside = IsInsidePlayerBounds(player2);

        if (p1Inside && p2Inside)
            return (player1.position + player2.position) * 0.5f;

        if (p1Inside)
            return player1.position;

        if (p2Inside)
            return player2.position;

        return transform.position;
    }

    public void SetPlayers(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;
    }

    bool IsInsidePlayerBounds(Transform t)
    {
        Vector3 p = t.position;
        return p.x >= playerMinBounds.x &&
               p.x <= playerMaxBounds.x &&
               p.y >= playerMinBounds.y &&
               p.y <= playerMaxBounds.y;
    }

    // =========================
    // CAMERA SHAKE
    // =========================

    void StartCameraShake(float magnitude, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(CameraShakeRoutine(magnitude, duration));
    }

    IEnumerator CameraShakeRoutine(float magnitude, float duration)
    {
        float timer = duration;

        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;

            Vector2 rnd = Random.insideUnitCircle * magnitude;
            shakeOffset = new Vector3(rnd.x, rnd.y, 0f);

            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }
}
