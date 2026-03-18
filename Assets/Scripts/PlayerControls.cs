using UnityEngine;
using UnityEngine.InputSystem;
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Camera")]
    public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0, 2, -4);
    public float cameraSmoothSpeed = 5f;

    [Header("Lock-On")]
    public float lockOnRange = 15f;
    public LayerMask enemyLayer;
    public Transform currentTarget;

    void Update()
    {
        HandleLockOnInput();
       // HandleMovement();
    }

    void LateUpdate()
    {
       // HandleCamera();
    }

    // ---------------- LOCK-ON ----------------
    void HandleLockOnInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentTarget is null)
                FindTarget();
            else
                currentTarget = null;
        }
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, lockOnRange, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                bestTarget = hit.transform;
            }
        }

        currentTarget = bestTarget;
    }

    // ---------------- MOVEMENT ----------------
   /* void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection;

        if (currentTarget is not null)
        {
            // LOCK-ON MOVEMENT (strafe around enemy)
            Vector3 toTarget = (currentTarget.position - transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, toTarget);

            moveDirection = toTarget * v + right * h;

            // Always face enemy
            Vector3 lookDir = currentTarget.position - transform.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            // FREE MOVEMENT (camera-relative)
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            moveDirection = forward * v + right * h;

            // Rotate toward movement
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        // Move player
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
    }

    // ---------------- CAMERA ----------------
    void HandleCamera()
    {
        if (currentTarget is not null)
        {
            // LOCK-ON CAMERA
            Vector3 direction = (transform.position - currentTarget.position).normalized;

            Vector3 desiredPosition = transform.position 
                                    + direction * 4f 
                                    + Vector3.up * 2f;

            cameraTransform.position = Vector3.Lerp(
                cameraTransform.position,
                desiredPosition,
                cameraSmoothSpeed * Time.deltaTime
            );

            Vector3 lookPoint = (transform.position + currentTarget.position) / 2f;
            cameraTransform.LookAt(lookPoint);
        }
        else
        {
            // NORMAL FOLLOW CAMERA
            Vector3 desiredPosition = transform.position 
                                    + transform.TransformDirection(cameraOffset);

            cameraTransform.position = Vector3.Lerp(
                cameraTransform.position,
                desiredPosition,
                cameraSmoothSpeed * Time.deltaTime
            );

            cameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
        }
    }*/
}
