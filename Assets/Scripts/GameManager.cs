using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    [Header("UI")]
    public HealthBar healthBarP1;
    public HealthBar healthBarP2;
    
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

        WaitForSeconds wait = new WaitForSeconds(0.2f);
        Health hp1 = player1.GetComponentInChildren<Health>();
        Health hp2 = player2.GetComponentInChildren<Health>();

        // =========================
        // UI CONNECTION
        // =========================
        healthBarP1.SetTarget(hp1);
        healthBarP2.SetTarget(hp2);
        PC pc1 = player1.GetComponentInChildren<PC>();
        PC pc2 = player2.GetComponentInChildren<PC>();

        Hitbox hb1 = player1.GetComponentInChildren<Hitbox>();
        Hitbox hb2 = player2.GetComponentInChildren<Hitbox>();

        hb1.SetOwner(pc1);
        hb2.SetOwner(pc2);
        // Asignada la camara
        cam.player1 = player1.transform;
        cam.player2 = player2.transform; 
    
    }
    
}
