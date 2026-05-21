using TMPro;
using UnityEngine;

public class UserDisplay : MonoBehaviour
{
    public FirebaseLogin firebaseLogin;
    public TMP_Text userText;

    void Start()
    {
        if (firebaseLogin == null)
        {
            Debug.LogError("FirebaseLogin no asignado");
            return;
        }

        var user = firebaseLogin.GetCurrentUser();

        if (user != null)
        {
            userText.text = "Logged as: " + user.Email;
        }
        else
        {
            userText.text = "No user logged";
        }
    }
}