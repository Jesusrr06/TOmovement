using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField usernameInput; // Input de email o usuario
    public TMP_InputField passwordInput; // Input de contraseña
    public TMP_Text errorText;           // Texto para mostrar errores o mensajes

    public GameObject playButton;  // Botón Play
    public GameObject quitButton;  // Botón Quit

    void Start()
    {
        // Inicializar estado de UI
        playButton.SetActive(true);
        quitButton.SetActive(true);
        errorText.text = "";
    }

    // =========================
    // Leer valores de los InputFields
    // =========================
    public void PrintInputValues()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log("Usuario: " + username);
        Debug.Log("Contraseña: " + password);

        // Mostrar en el errorText (opcional)
        errorText.text = $"Usuario: {username}\nContraseña: {password}";
    }

    // =========================
    // Método de login futuro (Firebase o local)
    // =========================
    /*
    public void OnLoginButtonPressed()
    {
        string email = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            errorText.text = "Por favor ingresa email y contraseña";
            return;
        }

        // Aquí iría la lógica de Firebase:
        // auth.SignInWithEmailAndPasswordAsync(email, password)...
    }
    */
}