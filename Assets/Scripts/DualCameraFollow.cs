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
    public float smoothSpeed = 5f;

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
        // Punto medio entre jugadores
        Vector3 center = (player1.position + player2.position) * 0.5f;

        // Distancia entre ellos
        float distance = Vector3.Distance(player1.position, player2.position);

        // Zoom dinámico
        float zoom = Mathf.Clamp(distance * distanceMultiplier, minZoom, maxZoom);

        // Posición de cámara
        Vector3 targetPosition = new Vector3(
            center.x,
            center.y + height,
            center.z - zoom
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * smoothSpeed
        );

        // Mirar al centro
        transform.LookAt(center + Vector3.up * 1f);
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