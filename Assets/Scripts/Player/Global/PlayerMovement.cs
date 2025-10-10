using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _jumpForce = 15;

    

    private void Movement()
    {
        
    }
}
