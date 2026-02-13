using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PLmovement : MonoBehaviour
{

    CharacterController cc;
    Vector3 velocity;
    float turnSmoothVelocity;
    public float speed = 0;
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
        void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);

    }
}
