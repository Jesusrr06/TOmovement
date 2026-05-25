using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// Controls main menu UI: start, quit and panel navigation.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;     // Panel principal con botones Play / Quit / Opciones
    [FormerlySerializedAs("LoadText")] public TMP_Text loadText;
    public GameObject registerPanel;
    public void Start()
    {
        loadText.SetText("");
        mainMenuPanel.SetActive(true);
        registerPanel.SetActive(false);
    }
    
    // =========================
    // Botón Play
    // =========================
    public void StartGame()
    {
        loadText.SetText("Loading characters...");
        
        Debug.Log("BOTON FUNCIONA");
        float forSeconds = 6.5f;
      
        // Cargar escena de juego (cambia "main" por tu escena)
        Invoke(nameof(LoadScene), forSeconds);
    }
    void LoadScene()
    {
        SceneManager.LoadScene("CharacterSelectScreen");
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
    public void ShowLoginPanel()
    {
        Debug.Log("BOTON FUNCIONA");
        mainMenuPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        Debug.Log("BOTON FUNCIONA");
        mainMenuPanel.SetActive(false);
        registerPanel.SetActive(true);
    }
    // =========================
    // Volver al panel principal
    // =========================

    public void BackToMainMenu()
    {
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}