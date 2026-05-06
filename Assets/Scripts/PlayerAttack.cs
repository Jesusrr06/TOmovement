using System.Collections;
using UnityEngine;

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
      
        if(Input.GetButton("Attack"))
        {
            animator.SetTrigger("Punching");
            
        }
    
        
     
        if (Input.GetButton("Shoot"))
        {
            Debug.Log("si");
            animator.SetTrigger("Shooting");
            //  movement.canMove = false;
            Shoot();
          //   movement.canMove = true;
                     
        }
      
        if(Input.GetButton("HeavyAttack"))
        {
            animator.SetTrigger("Kicking");
            
        }
        if(Input.GetButton("Block"))
        {
            animator.SetBool("ISGuarding", Input.GetKeyDown("IsGuarding"));
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

