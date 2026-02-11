using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensx;
    public float sensy;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X")* Time.deltaTime*sensx;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensy;
        yRotation +=  mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xrotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(xrotation, yrotation, 0);
        orientation = Quaternion.Euler(0, yrotation, 0);

    }
}
