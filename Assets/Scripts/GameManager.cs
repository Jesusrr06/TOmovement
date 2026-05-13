using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    public GameObject[] playerPrefabs;
    
    [Header("UI")]
    public HealthBar healthBarP1;
    public HealthBar healthBarP2;
    public GameObject gameOverPanel; // Panel con botones Jugar otra vez / Salir al menú
     public TextMeshProUGUI  winnerText;

    [Header("Spawn")]
    public Transform spawnP1;
    public Transform spawnP2;
    public OrbitalCamera cam;

    private GameObject player1;
    private GameObject player2;

    void Start()
    {
        gameOverPanel.SetActive(false); // Oculta el panel al inicio
        StartCoroutine(SpawnPlayers());
    }

    IEnumerator SpawnPlayers()
    {
        // =========================
        // PLAYER 1
        // =========================
        player1 = new GameObject("Player1");
        player1.transform.position = spawnP1.position;
        player1.transform.rotation = spawnP1.rotation;

        GameObject model1 = Instantiate(playerPrefabs[GameData.Player1Character]);
        model1.transform.SetParent(player1.transform);
        model1.transform.localPosition = Vector3.zero;
        model1.transform.localRotation = Quaternion.identity;
        player1.tag = "Player1";

        // =========================
        // PLAYER 2
        // =========================
        player2 = new GameObject("Player2");
        player2.transform.position = spawnP2.position;
        player2.transform.rotation = spawnP2.rotation;

        GameObject model2 = Instantiate(playerPrefabs[GameData.Player2Character]);
        model2.transform.SetParent(player2.transform);
        model2.transform.localPosition = Vector3.zero;
        model2.transform.localRotation = Quaternion.identity;
        player2.tag = "Player2";

        yield return null;
        WaitForSeconds wait = new WaitForSeconds(0.2f);

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

        // =========================
        // Hitboxes
        // =========================
        PC pc1 = player1.GetComponentInChildren<PC>();
        PC pc2 = player2.GetComponentInChildren<PC>();

        Hitbox hb1 = player1.GetComponentInChildren<Hitbox>();
        Hitbox hb2 = player2.GetComponentInChildren<Hitbox>();

        hb1.SetOwner(pc1);
        hb2.SetOwner(pc2);

        // =========================
        // Camara
        // =========================
        cam.player1 = player1.transform;
        cam.player2 = player2.transform;
    }

    // =========================
    // Fin del combate
    // =========================
    private void OnPlayerDeath(PC deadPlayer)
    {
        // Determinar quién ganó
        string winner = (deadPlayer.gameObject.name == "Player1") ? "Player 2" : "Player 1";

        // Mostrar panel Game Over
        gameOverPanel.SetActive(true);
        winnerText.text = $"{winner} ganó el combate!";

        // Desactivar controles de ambos jugadores
        player1.GetComponentInChildren<PC>().enabled = false;
        player2.GetComponentInChildren<PC>().enabled = false;

        // Desactivar hitboxes
        player1.GetComponentInChildren<Hitbox>().DisableAll();
        player2.GetComponentInChildren<Hitbox>().DisableAll();
      
        

        // Pausar tiempo si quieres animaciones
        Time.timeScale = 0f;
    }
   

    
    // =========================
    // Botones UI
    // =========================
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Cambia por tu escena de menú
    }
}