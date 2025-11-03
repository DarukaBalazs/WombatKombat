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

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
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
        Vector3 newPosition = centerPoint;

        newPosition.z = transform.position.z;

        // Clamp kamera mozgása a határokon belül
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float distance = Vector3.Distance(player1.position, player2.position);
        float newZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    Vector3 GetCenterPoint()
    {
        return (player1.position + player2.position) / 2f;
    }

    public void SetPlayers(Transform player1,  Transform player2)
    {
        this.player1 = player1;
        this.player2 = player2;
    }
}