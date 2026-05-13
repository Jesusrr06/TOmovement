using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;     // Panel principal con botones Play / Quit / Opciones
    public GameObject optionsPanel;      // Otro panel de opciones, por ejemplo
    public void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }
    
    // =========================
    // Botón Play
    // =========================
    public void StartGame()
    {
        Debug.Log("BOTON FUNCIONA");

        // Cargar escena de juego (cambia "main" por tu escena)
        SceneManager.LoadScene("main");
    }

    // =========================
    // Botón Quit
    // =========================
    public void QuitGame()
    {
        Debug.Log("BOTON FUNCIONA");

        // Salir de la aplicación
        Application.Quit();
    }

    // =========================
    // Mostrar panel de opciones
    // =========================
    public void ShowOptionsPanel()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    // =========================
    // Volver al panel principal
    // =========================
    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}