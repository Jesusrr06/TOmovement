using System.Collections;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public float height = 10f;
    public float distance = 10f;
    public float rotationSpeed = 10f;

    private float angle;

    void Start()
    {
        StartCoroutine(FindPlayers());
    }

    void LateUpdate()
    {
        if (!player1 || !player2) return;

        Vector3 center = (player1.position + player2.position) / 2f;

        angle += rotationSpeed * Time.deltaTime;

        float rad = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Sin(rad) * distance,
            height,
            Mathf.Cos(rad) * distance
        );

        transform.position = center + offset;
        transform.LookAt(center);
    }

    IEnumerator FindPlayers()
    {
        while (player1 is null || player2 is null)
        {
            PC[] players = Object.FindObjectsByType<PC>(FindObjectsSortMode.None);

            foreach (var p in players)
            {
                if (p.playerId == 1) player1 = p.transform;
                if (p.playerId == 2) player2 = p.transform;
            }

            yield return null;
        }
    }
}