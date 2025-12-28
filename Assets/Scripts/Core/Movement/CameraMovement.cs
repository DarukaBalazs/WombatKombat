using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Camera Settings")]
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float zoomLimiter = 20f;

    [Header("Camera Bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    Vector3 velocity;
    Camera cam;

    // --- SHAKE ---
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
        targetPos.z = transform.position.z;

        targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
        targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

        Vector3 basePos = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        // ADDITÍV SHAKE (NEM RESETEL)
        transform.position = basePos + shakeOffset;
    }

    void Zoom()
    {
        float distance = Vector3.Distance(player1.position, player2.position);
        float newZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    Vector3 GetCenterPoint()
    {
        return (player1.position + player2.position) * 0.5f;
    }

    public void SetPlayers(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;
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
