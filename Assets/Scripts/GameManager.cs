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

      
        if (player1 is null || player2 is null)
        {
            Debug.LogError("player1 or player2 root not assigned in GameManager.");
            yield break;
        }

        // Ensure valid indices (use available prefab lengths)
        if (GameData.Player1Character < 0 || GameData.Player1Character >= playerPrefabsP1.Length)
        {
            int old = GameData.Player1Character;
            GameData.Player1Character = Random.Range(0, playerPrefabsP1.Length);
            Debug.LogWarning($"Player1Character invalid ({old}) — randomized to {GameData.Player1Character}");
        }
        if (GameData.Player2Character < 0 || GameData.Player2Character >= playerPrefabsP2.Length)
        {
            int old = GameData.Player2Character;
            GameData.Player2Character = Random.Range(0, playerPrefabsP2.Length);
            Debug.LogWarning($"Player2Character invalid ({old}) — randomized to {GameData.Player2Character}");
        }

        player1.transform.position = spawnP1.position;
        player1.transform.rotation = spawnP1.rotation;

        // Safety: temporarily keep movement components disabled until positioned to avoid immediate physics/input moving the character out of bounds.
        PlayerMovement pm1 = null, pm2 = null;
        CharacterController cc1 = null, cc2 = null;

        Debug.Log($"Spawning P1 index={GameData.Player1Character} prefab={playerPrefabsP1[GameData.Player1Character].name}");
        GameObject model1 = Instantiate(playerPrefabsP1[GameData.Player1Character], player1.transform, true);
        if (model1 is null)
        {
            Debug.LogError("Instantiate returned null for P1");
        }
        else
        {
            model1.transform.localPosition = Vector3.zero;
            model1.transform.localRotation = Quaternion.identity;
            model1.tag = "Player1";
            model1.name = model1.name + "_P1";
            player1.tag = "Player1";
            Debug.Log($"P1 instantiated: {model1.name}, player1 children={player1.transform.childCount}");

            // Disable movement/character controller immediately to prevent any Awake/Start movement from relocating the model before positioning is final.
            pm1 = model1.GetComponentInChildren<PlayerMovement>();
            cc1 = model1.GetComponentInChildren<CharacterController>();
            if (pm1 != null) pm1.enabled = false;
            if (cc1 != null) cc1.enabled = false;
        }

        // =========================
        // PLAYER 2
        // =========================
        player2.transform.position = spawnP2.position;
        player2.transform.rotation = spawnP2.rotation;

        Debug.Log($"Spawning P2 index={GameData.Player2Character} prefab={playerPrefabsP2[GameData.Player2Character].name}");
        GameObject model2 = Instantiate(playerPrefabsP2[GameData.Player2Character], player2.transform, true);
        if (model2 is null)
        {
            Debug.LogError("Instantiate returned null for P2");
        }
        else
        {
            model2.tag = "Player2";
            model2.transform.localPosition = Vector3.zero;
            model2.transform.localRotation = Quaternion.identity;
            model2.name = model2.name + "_P2";
            player2.tag = "Player2";
            Debug.Log($"P2 instantiated: {model2.name}, player2 children={player2.transform.childCount}");

            // Disable movement/character controller until repositioning completes.
            pm2 = model2.GetComponentInChildren<PlayerMovement>();
            cc2 = model2.GetComponentInChildren<CharacterController>();
            if (pm2 != null) pm2.enabled = false;
            if (cc2 != null) cc2.enabled = false;
        }

        yield return new WaitForSeconds(0.2f);

        // Re-enable movement components now that positioning is stable.
        if (cc1 != null) cc1.enabled = true;
        if (pm1 != null) pm1.enabled = true;
        if (cc2 != null) cc2.enabled = true;
        if (pm2 != null) pm2.enabled = true;

        // =========================
        // Health
        // =========================
        Health hp1 = player1.GetComponentInChildren<Health>();
        Health hp2 = player2.GetComponentInChildren<Health>();

        if (hp1 == null) Debug.LogError("Health component not found under player1 after instantiation.");
        if (hp2 == null) Debug.LogError("Health component not found under player2 after instantiation.");

        // Conectar UI (solo si existen)
        if (hp1 != null) healthBarP1.SetTarget(hp1);
        if (hp2 != null) healthBarP2.SetTarget(hp2);

        // Suscribirse al evento de muerte (si existen)
        if (hp1 != null) hp1.OnDeath += OnPlayerDeath;
        if (hp2 != null) hp2.OnDeath += OnPlayerDeath;
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