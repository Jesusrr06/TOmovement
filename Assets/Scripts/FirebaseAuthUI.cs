using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Simple UI bridge between Unity UI and FirebaseLogin
public class FirebaseAuthUI : MonoBehaviour
{
    [Header("References")]
    public FirebaseLogin firebaseLogin;
    public UserDisplay userDisplay;

    [Header("Inputs")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    [Header("Buttons")]
    public Button registerButton;
    public Button loginButton;
    public Button logoutButton;

    [Header("Feedback")]
    public TMP_Text feedbackText;

    void Reset()
    {
        if (firebaseLogin == null)
            firebaseLogin = FindAnyObjectByType<FirebaseLogin>();

        if (userDisplay == null)
            userDisplay = FindAnyObjectByType<UserDisplay>();
    }

    void OnEnable()
    {
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);

        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);

        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogoutClicked);
    }

    void OnDisable()
    {
        if (registerButton != null)
            registerButton.onClick.RemoveListener(OnRegisterClicked);

        if (loginButton != null)
            loginButton.onClick.RemoveListener(OnLoginClicked);

        if (logoutButton != null)
            logoutButton.onClick.RemoveListener(OnLogoutClicked);
    }

    void OnRegisterClicked()
    {
        if (!EnsureFirebaseReady())
            return;

        firebaseLogin.Register(GetEmail(), GetPassword(), (ok, msg) =>
        {
            SetFeedback(ok
                ? " Registrado: " + msg
                : " Error registro: " + msg);

            RefreshUserDisplay();
        });
    }

    void OnLoginClicked()
    {
        if (!EnsureFirebaseReady())
            return;

        firebaseLogin.Login(GetEmail(), GetPassword(), (ok, msg) =>
        {
            SetFeedback(ok
                ? " Login correcto: " + msg
                : " Error login: " + msg);

            RefreshUserDisplay();
        });
    }

    void OnLogoutClicked()
    {
        if (firebaseLogin == null)
        {
            SetFeedback(" FirebaseLogin no asignado");
            return;
        }

        firebaseLogin.Logout();

        RefreshUserDisplay();

        SetFeedback("Sesión cerrada");
    }

    bool EnsureFirebaseReady()
    {
        if (firebaseLogin == null)
        {
            SetFeedback(" FirebaseLogin no asignado");
            return false;
        }

        if (!firebaseLogin.FirebaseReady)
        {
            SetFeedback(" Firebase no está listo aún");
            return false;
        }

        return true;
    }

    void RefreshUserDisplay()
    {
        if (userDisplay != null)
        {
            userDisplay.ShowUserStatus();
        }
    }

    string GetEmail()
    {
        return emailInput != null
            ? emailInput.text.Trim()
            : string.Empty;
    }

    string GetPassword()
    {
        return passwordInput != null
            ? passwordInput.text
            : string.Empty;
    }

    void SetFeedback(string msg)
    {
        if (feedbackText != null)
            feedbackText.text = msg;

        Debug.Log(msg);
    }
}