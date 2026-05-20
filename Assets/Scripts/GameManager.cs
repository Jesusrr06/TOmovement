using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// Spawns players, wires UI and camera, and handles round end / restart logic.
/// </summary>
public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("playerPrefabs")] [Header("Players")]
    public GameObject[] playerPrefabsP1;
    public GameObject[] playerPrefabsP2;
    public GameObject player1;
    public GameObject player2;
    [Header("UI")]
    public HealthBar healthBarP1;
    public HealthBar healthBarP2;
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;

    [Header("Spawn")]
    public Transform spawnP1;
    public Transform spawnP2;

    void Start()
    {
        gameOverPanel.SetActive(false);
        StartCoroutine(SpawnPlayers());
    }

    /// <summary>
    /// Instantiates player root GameObjects, attaches character models, wires Health and UI,
    /// and assigns camera targets. Runs as a coroutine to allow frames for prefab initialization.
    /// </summary>
    private   IEnumerator SpawnPlayers()
    {
        player1.transform.position = spawnP1.position;
        player1.transform.rotation = spawnP1.rotation;
        if (GameData.Player1Character < 0 || GameData.Player1Character > 1)
        {
            GameData.Player1Character = Random.Range(0, 2);

        }

        GameObject model1 = Instantiate(playerPrefabsP1[GameData.Player1Character], player1.transform, true);
        model1.transform.localPosition = Vector3.zero;
        model1.transform.localRotation = Quaternion.identity;
        model1.tag = "Player1";
        player1.tag = "Player1";

        // =========================
        // PLAYER 2
        // =========================
        player2.transform.position = spawnP2.position;
        player2.transform.rotation = spawnP2.rotation;
        if (GameData.Player2Character < 0 || GameData.Player2Character > 1)
        {
            GameData.Player2Character = Random.Range(0, 2);

        }
        GameObject model2 = Instantiate(playerPrefabsP2[GameData.Player2Character], player2.transform, true);
        model2.tag = "Player2";
        model2.transform.localPosition = Vector3.zero;
        model2.transform.localRotation = Quaternion.identity;
        player2.tag = "Player2";

        yield return new WaitForSeconds(0.2f);

        // =========================
        // Health
        // =========================
        Health hp1 = player1.GetComponentInChildren<Health>();
        Health hp2 = player2.GetComponentInChildren<Health>();

        // Conectar UI
        healthBarP1.SetTarget(hp1);
        healthBarP2.SetTarget(hp2);

        // Suscribirse al evento de muerte
        hp1.OnDeath += OnPlayerDeath;
        hp2.OnDeath += OnPlayerDeath; 
    }
    

    /// <summary>
    /// Callback invoked when a player's Health fires OnDeath. Shows game over UI and freezes the game.
    /// </summary>
    /// <param name="deadPlayer">PlayerMovement of the dead player.</param>
    private void OnPlayerDeath(PlayerMovement deadPlayer)
    {
        bool p1Died = deadPlayer == player1.GetComponentInChildren<PlayerMovement>();
        string winner = p1Died ? "Player 2" : "Player 1";

        gameOverPanel.SetActive(true);
        winnerText.text = $"{winner} ganó el combate!";

        player1.GetComponentInChildren<PlayerMovement>().enabled = false;
        player2.GetComponentInChildren<PlayerMovement>().enabled = false;

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Restarts the current scene and resumes normal timescale.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads the main menu scene and resumes timescale.
    /// </summary>
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}