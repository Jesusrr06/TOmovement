using UnityEngine;

public class PC : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    [SerializeField]
    private float movementSpeed, rotationSpeed, jumpSpeed,gravity;
   // public bool canMove = true;

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

//Debug.Log(canMove);

        Vector3 inputMovement = transform.forward * (Input.GetAxisRaw("Vertical") * movementSpeed);

        transform.Rotate(Vector3.up * (Input.GetAxisRaw("Horizontal") * rotationSpeed));
        Debug.Log(characterController.isGrounded);
        Jump();

        
        Vector3 finalMovement = inputMovement + movementDirection;
     //   if (!canMove)
       // {
         characterController.Move(finalMovement * Time.deltaTime);
      //  }

        animator.SetBool("IsJumping", !characterController.isGrounded);
         
        animator.SetBool("IsFalling", !characterController.isGrounded);

      

        animator.SetBool("IsRunning", Input.GetAxisRaw("Vertical")!=0);
        
    }


    void Jump()
    {   if (Input.GetButtonDown("Jump") && characterController.isGrounded)
             {      
                 movementDirection.y = jumpSpeed;    

             }
             movementDirection.y-=gravity* Time.deltaTime;
    }

    
     
}
