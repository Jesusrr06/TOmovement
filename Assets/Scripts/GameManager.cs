using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] playerPrefabs;

    public Transform spawnP1;
    public Transform spawnP2;
    public OrbitalCamera cam;

    void Start()
    {
        StartCoroutine(SpawnPlayers());
    }

    IEnumerator SpawnPlayers()
    {
        // =========================
        // PLAYER 1
        // =========================
        GameObject player1 = new GameObject("Player1");
        player1.transform.position = spawnP1.position;
        player1.transform.rotation = spawnP1.rotation;

        GameObject model1 = Instantiate(
            playerPrefabs[GameData.Player1Character]
        );

        model1.transform.SetParent(player1.transform);
        model1.transform.localPosition = Vector3.zero;
        model1.transform.localRotation = Quaternion.identity;

        player1.tag = "Player1";


        // =========================
        // PLAYER 2
        // =========================
        GameObject player2 = new GameObject("Player2");
        player2.transform.position = spawnP2.position;
        player2.transform.rotation = spawnP2.rotation;

        GameObject model2 = Instantiate(
            playerPrefabs[GameData.Player2Character]
        );

        model2.transform.SetParent(player2.transform);
        model2.transform.localPosition = Vector3.zero;
        model2.transform.localRotation = Quaternion.identity;

        player2.tag = "Player2";

        yield return null;
   

        // 🔥 AQUÍ LA MAGIA
        cam.player1 = player1.transform;
        cam.player2 = player2.transform;
    }
    
}
