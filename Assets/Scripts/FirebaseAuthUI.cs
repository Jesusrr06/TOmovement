using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Simple UI bridge between Unity UI and FirebaseLogin
public class FirebaseAuthUI : MonoBehaviour
{
    [Header("References")]
    public FirebaseLogin firebaseLogin; // assign this in inspector (or let Reset find one)
    public TMP_InputField  emailInput;
    public TMP_InputField  passwordInput;

    [Header("Controls")]
    public Button registerButton;
    public Button loginButton;
    public Button logoutButton;

    [Header("Feedback")]
    public TMP_Text  feedbackText;

    void Reset()
    {
        if (firebaseLogin == null)
            firebaseLogin = FindAnyObjectByType<FirebaseLogin>();
    }

    void OnEnable()
    {
        if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClicked);
        if (loginButton != null) loginButton.onClick.AddListener(OnLoginClicked);
        if (logoutButton != null) logoutButton.onClick.AddListener(OnLogoutClicked);
    }

    void OnDisable()
    {
        if (registerButton != null) registerButton.onClick.RemoveListener(OnRegisterClicked);
        if (loginButton != null) loginButton.onClick.RemoveListener(OnLoginClicked);
        if (logoutButton != null) logoutButton.onClick.RemoveListener(OnLogoutClicked);
    }

    void OnRegisterClicked()
    {
        if (!EnsureLogin()) return;
        firebaseLogin.Register(GetEmail(), GetPassword(), (ok, msg) =>
        {
            SetFeedback(ok ? "Registrado: " + msg : "Error registro: " + msg);
        });
    }

    void OnLoginClicked()
    {
        if (!EnsureLogin()) return;
        firebaseLogin.Login(GetEmail(), GetPassword(), (ok, msg) =>
        {
            SetFeedback(ok ? "Login correcto: " + msg : "Error login: " + msg);
        });
    }

    void OnLogoutClicked()
    {
        if (firebaseLogin == null) { SetFeedback("FirebaseLogin no asignado"); return; }
        firebaseLogin.Logout();
        SetFeedback("Sesión cerrada");
    }

    bool EnsureLogin()
    {
        if (firebaseLogin == null)
        {
            SetFeedback("FirebaseLogin no asignado");
            return false;
        }
        if (!firebaseLogin.FirebaseReady)
        {
            SetFeedback("Firebase no está listo aún");
            return false;
        }
        return true;
    }

    string GetEmail() => emailInput != null ? emailInput.text.Trim() : string.Empty;
    string GetPassword() => passwordInput != null ? passwordInput.text : string.Empty;

    void SetFeedback(string msg)
    {
        if (feedbackText != null)
            feedbackText.text = msg;
        else
            Debug.Log(msg);
    }
}