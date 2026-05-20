using System.Collections;
using UnityEngine;

/// <summary>
/// Camera that follows two players: centers between them and adjusts zoom based on distance.
/// </summary>
public class DualCameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    [Header("Follow Settings")]
    public float height = 8f;
    public float distanceMultiplier = 1.2f;

    [Header("Zoom Limits")]
    public float minZoom = 6f;
    public float maxZoom = 20f;

    [Header("Smoothing")]
    public float smoothTime = 0.2f;
    public float rotationSpeed = 5f;

    [Header("Camera Angle")]
    public float angle = 0f;

    // internal smoothing velocity
    private Vector3 smoothVelocity;

    private void Start()
    {
        StartCoroutine(FindPlayers());
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        UpdateCamera();
    }

    void UpdateCamera()
    {
        // Middle point between players
        Vector3 center = (player1.position + player2.position) * 0.5f;

        // Distance between them
        float distance = Vector3.Distance(player1.position, player2.position);

        // Desired distance based on players separation
        float desiredDistance = Mathf.Clamp(distance * distanceMultiplier, minZoom, maxZoom);

        // Target to look at (slightly above center)
        Vector3 lookTarget = center + Vector3.up * 1f;

        // Use a fixed offset direction (relative to world forward) so camera stays behind at an angle
        Vector3 offsetDir = Quaternion.Euler(0f, angle, 0f) * Vector3.back;

        // Compute target position using desired distance and height
        Vector3 targetPosition = lookTarget + offsetDir * desiredDistance;
        targetPosition.y = center.y + height;

        // Smoothly move camera towards target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, smoothTime);

        // Smoothly rotate to look at the center
        Quaternion targetRot = Quaternion.LookRotation((lookTarget - transform.position).normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    private IEnumerator FindPlayers()
    {
        while (player1 is null || player2 is null)
        {
            PlayerMovement[] players = FindObjectsByType<PlayerMovement>();

            foreach (PlayerMovement p in players)
            {
                if (p.playerId == 1) player1 = p.transform;
                if (p.playerId == 2) player2 = p.transform;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}