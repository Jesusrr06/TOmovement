using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;

    void Start()
    {
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        GameObject p1 = Instantiate(playerPrefab, spawn1.position, spawn1.rotation);
        GameObject p2 = Instantiate(playerPrefab, spawn2.position, spawn2.rotation);

        PlayerInput input1 = p1.GetComponent<PlayerInput>();
        PlayerInput input2 = p2.GetComponent<PlayerInput>();

        input1.SwitchCurrentActionMap("Player1");
        input2.SwitchCurrentActionMap("Player2");

        p1.name = "Player1";
        p2.name = "Player2";
    }
}