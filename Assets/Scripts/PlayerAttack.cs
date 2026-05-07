using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{ 
    public GameObject projectilePrefab;
    public Transform firePoint;
    private CharacterController characterController;
    private Animator animator;
   // public PC movement;

    void Start()
    {  characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
     //   movement = GetComponent<PC>();
    }

    // Update is called once per frame
    void Update()
    {
      
        if(Keyboard.current.eKey.isPressed)
        {
            animator.SetTrigger("Punching");
            
        }
    
        
     
        if (Keyboard.current.rKey.isPressed)
        {
            Debug.Log("si");
            animator.SetTrigger("Shooting");
            //  movement.canMove = false;
            Shoot();
          //   movement.canMove = true;
                     
        }
      
        if(Keyboard.current.qKey.isPressed)
        {
            animator.SetTrigger("Kicking");
            
        }
        if(Keyboard.current.fKey.isPressed)
        {
            animator.SetBool("IsGuarding", Keyboard.current.fKey.isPressed);
        }
        
    }

    


    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    IEnumerator AttackRoutine()
    {
        animator.SetTrigger("Punching");
        
            
        
        
            
        
        animator.SetTrigger("Kicking");

        yield return new WaitForSeconds(0.5f);

      //  movement.canMove = true;
    }


    
}

