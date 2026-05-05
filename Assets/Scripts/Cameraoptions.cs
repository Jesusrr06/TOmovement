using UnityEngine;
using UnityEngine.Serialization;

public class Cameraoptions : MonoBehaviour
{
    public GameObject followtransform;
    public Transform player; // Reference to the player character
     public Vector3 offset;  // Offset from the player
     public float rotationSpeed = 1f; // Smoothness of rotation

    [FormerlySerializedAs("Player2")] public GameObject lockontoTag;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (followtransform.CompareTag("Player1"))
        {
        

            followtransform = GameObject.FindGameObjectWithTag("Player1");
            lockontoTag = GameObject.FindGameObjectWithTag("Player2");
        }
        if (followtransform.CompareTag("Player2"))
        {
        

            followtransform = GameObject.FindGameObjectWithTag("Player2");
            lockontoTag = GameObject.FindGameObjectWithTag("Player1");
        }

    }

 
  
    void LateUpdate()
    {
        // Follow the player's position
        transform.position = player.position + offset;

        // Rotate the camera to match the player's rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, rotationSpeed * Time.deltaTime);
    }
}
