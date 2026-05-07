using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player Prefabs")]
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;

    void Start()
    {
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        Instantiate(player1Prefab, new Vector3(0, 0, 0), Quaternion.identity);
        player1Prefab.tag = "Player1";
        Instantiate(player2Prefab, new Vector3(2, 0, 0), Quaternion.identity);
player2Prefab.tag = "Player2";
player2Prefab.name = "Player2";

        
    }
}