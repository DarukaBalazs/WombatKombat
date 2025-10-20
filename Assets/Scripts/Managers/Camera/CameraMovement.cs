using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform player1;
    Transform player2;

    public Camera cam;
    public float minSizeY = 5f;
    public float maxSizeY = 12f;
    public void SetPlayers(Transform player1, Transform player2)
    {
        this.player1 = player1;
        this.player2 = player2;
    }

    private void SetCameraPosition()
    {
        if (player1 == null || player2 == null) return;
        
        Vector3 middle = (player1.position + player2.position) * 0.5f;

        transform.position = new Vector3(middle.x,middle.y, transform.position.z); 
    }

    private void SetCameraSize()
    {
        if (player1 == null || player2 == null) return;

        float minSizeX = minSizeY * Screen.width / Screen.height;
        float maxSizeX = maxSizeY * Screen.width / Screen.height;

        float width = Mathf.Abs(player1.position.x - player2.position.x);
        float height = Mathf.Abs(player1.position.y - player2.position.y) * 0.5f;

        float camSizeX = Mathf.Min(maxSizeX,Mathf.Max(width, minSizeX));

        cam.orthographicSize = Mathf.Max(height,
            camSizeX * Screen.height / Screen.width, minSizeY);

    }

    private void Update()
    {
        SetCameraPosition();
        SetCameraSize();
    }
}
