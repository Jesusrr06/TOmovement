using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

public class UserDisplay : MonoBehaviour
{
    [Header("Firebase")]
    public FirebaseLogin firebaseLogin;

    [Header("UI")]
    public TMP_Text displayTextTMP;

    void Start()
    {
        ShowUserStatus();
    }

    private void ShowUserStatus()
    {
        if (firebaseLogin == null)
        {
            SetText("FirebaseLogin no asignado");
            return;
        }

        FirebaseUser user = firebaseLogin.GetCurrentUser();

        if (user != null)
        {
            string email = string.IsNullOrEmpty(user.Email)
                ? "Usuario sin email"
                : user.Email;

            SetText("✅ Logueado\n" + email);
        }
        else
        {
            SetText("❌ No hay ninguna cuenta iniciada");
        }
    }

    void SetText(string message)
    {
        if (displayTextTMP != null)
            displayTextTMP.text = message;
        
        Debug.Log(message);
    }
}