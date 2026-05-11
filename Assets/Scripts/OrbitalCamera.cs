using System.Collections;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    [Header("Camera Settings")]
    public float height = 10f;
    public float minDistance = 10f;
    public float maxDistance = 30f;
    public float rotationSpeed = 10f;
    public float zoomSpeed = 2f;
    
    private float _currentDistance;

    private float _angle;

    private void Start()
    {
        _currentDistance = minDistance;

        StartCoroutine(FindPlayers());
    }

    private void LateUpdate()
    {if (player1 == null || player2 == null)
        {
            FindPlayersNow();
            return;
        }

        // Midpoint between players
        Vector3 center = (player1.position + player2.position) * 0.5f;

        //this da distance between players
        float playersApart = Vector3.Distance(player1.position, player2.position);

        float targetDistance = Mathf.Clamp(
            playersApart,
            minDistance,
            maxDistance
        );
        // Camera does zoom based on separation

        _currentDistance = Mathf.Lerp(
            _currentDistance,
            targetDistance,
            Time.deltaTime * zoomSpeed
        );

        // Rotate around players
        _angle += rotationSpeed * Time.deltaTime;

        float rad = _angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Sin(rad) * _currentDistance,
            height,
            Mathf.Cos(rad) * _currentDistance
        );

        transform.position = center + offset;

        transform.LookAt(center);
    }

    private IEnumerator FindPlayers()
    {
        while (player1 is null || player2 is null)
        {
            PC[] players = FindObjectsByType<PC>();

            foreach (var p in players)
            {
                if (p.playerId == 1) player1 = p.transform;
                if (p.playerId == 2) player2 = p.transform;
            }

            yield return new WaitForSeconds(0.2f);
            ;
        }
    }
    void FindPlayersNow()
    {
        PC[] players = FindObjectsByType<PC>();

        foreach (PC p in players)
        {
            if (p.playerId == 1)
                player1 = p.transform;

            if (p.playerId == 2)
                player2 = p.transform;
        }
    }
}