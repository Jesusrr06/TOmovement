using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Spawns players, wires UI and camera, and handles round end / restart logic.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Players")]
    public GameObject[] playerPrefabs;

    [Header("UI")]
    public HealthBar healthBarP1;
    public HealthBar healthBarP2;
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;

    [Header("Spawn")]
    public Transform spawnP1;
    public Transform spawnP2;
    public DualCameraFollow cam;

    private GameObject _player1;
    private GameObject _player2;

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
         _player1 = new GameObject("Player1");
        _player1.transform.position = spawnP1.position;
        _player1.transform.rotation = spawnP1.rotation;

        GameObject model1 = Instantiate(playerPrefabs[GameData.Player1Character], _player1.transform, true);
        model1.transform.localPosition = Vector3.zero;
        model1.transform.localRotation = Quaternion.identity;
        model1.tag = "Player1";
        _player1.tag = "Player1";

        // =========================
        // PLAYER 2
        // =========================
        _player2 = new GameObject("Player2");
        _player2.transform.position = spawnP2.position;
        _player2.transform.rotation = spawnP2.rotation;

        GameObject model2 = Instantiate(playerPrefabs[GameData.Player2Character], _player2.transform, true);
        model2.tag = "Player2";
        model2.transform.localPosition = Vector3.zero;
        model2.transform.localRotation = Quaternion.identity;
        _player2.tag = "Player2";

        yield return null;
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        // =========================
        // Health
        // =========================
        Health hp1 = _player1.GetComponentInChildren<Health>();
        Health hp2 = _player2.GetComponentInChildren<Health>();

        // Conectar UI
        healthBarP1.SetTarget(hp1);
        healthBarP2.SetTarget(hp2);

        // Suscribirse al evento de muerte
        hp1.OnDeath += OnPlayerDeath;
        hp2.OnDeath += OnPlayerDeath;
        // =========================
        // Camara
        // =========================
        cam.player1 = _player1.transform;
        cam.player2 = _player2.transform;
    }
    

    /// <summary>
    /// Callback invoked when a player's Health fires OnDeath. Shows game over UI and freezes the game.
    /// </summary>
    /// <param name="deadPlayer">PlayerMovement of the dead player.</param>
    private void OnPlayerDeath(PlayerMovement deadPlayer)
    {
        bool p1Died = deadPlayer == _player1.GetComponentInChildren<PlayerMovement>();
        string winner = p1Died ? "Player 2" : "Player 1";

        gameOverPanel.SetActive(true);
        winnerText.text = $"{winner} ganó el combate!";

        _player1.GetComponentInChildren<PlayerMovement>().enabled = false;
        _player2.GetComponentInChildren<PlayerMovement>().enabled = false;

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