using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jump=5f;  
    private PlayerInputController playerInputController;
    private GroundController groundController;
    private Rigidbody rb;
    private bool jumpTriggered;
    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
       groundController = GetComponent<GroundController>();
        rb = GetComponent<Rigidbody>();
        groundController = GetComponent<GroundController>();

         playerInputController.OnJumpButtonPressed += JumpButtonPressed;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(playerInputController.MovementInputVector.x, 
             0,
             playerInputController.MovementInputVector.y)  * speed;
      
        velocity.y= rb.linearVelocity.y;
         if (jumpTriggered)
         {
             velocity.y = jump;
             jumpTriggered = false;
         }
         rb.linearVelocity = velocity;
    }
    private void JumpButtonPressed()
    {
        if (groundController.IsGrounded)
        {
            jumpTriggered = true;
        } 
    }
}
