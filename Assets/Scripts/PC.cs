using UnityEngine;

public class PC : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    [SerializeField]
    private float movementSpeed, rotationSpeed, jumpSpeed,gravity;

    private Vector3 movementDirection= Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputMovement= transform.forward * (Input.GetAxisRaw("Vertical") * movementSpeed);
        characterController.Move(inputMovement * Time.deltaTime);
        
        transform.Rotate(Vector3.up * (Input.GetAxisRaw("Horizontal") * rotationSpeed) );
       Debug.Log(characterController.isGrounded);
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            movementDirection.y = jumpSpeed;
            Debug.Log("isjump should works");
        }
        movementDirection.y-=gravity* Time.deltaTime;
        characterController.Move(movementDirection * Time.deltaTime);
        
        animator.SetBool("IsRunning", Input.GetAxisRaw("Vertical")!=0);            
        animator.SetBool("IsJumping", !characterController.isGrounded);            

    }
}
