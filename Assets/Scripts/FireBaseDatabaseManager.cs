using System;
using System.Collections;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
    public TMP_InputField Username;
    public TMP_InputField Password;
    public TMP_InputField Usernameupdate;
    public TMP_InputField Usernamedelete;

    public TMP_Text nametext;

    private string userID;
    private DatabaseReference db;

    void Start()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Firebase Database conectada correctamente");
    }

    // 🔥 CREATE USER (SOLO NAME)
    public void CreateUser()
    {
        userID = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

        if (string.IsNullOrEmpty(userID))
        {
            Debug.LogError("Usuario no logueado");
            return;
        }

        db.Child("user")
          .Child(userID)
          .Child("name")
          .SetValueAsync(Username.text);
    }

    // 🔥 GET NAME
    public IEnumerator GetName(Action<string> onCallback)
    {
        userID = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

        if (string.IsNullOrEmpty(userID))
        {
            onCallback?.Invoke("N/A");
            yield break;
        }

        var task = db.Child("user")
                     .Child(userID)
                     .Child("name")
                     .GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("Error leyendo name: " + task.Exception);
            onCallback?.Invoke("Error");
            yield break;
        }

        string name = task.Result.Value?.ToString() ?? "N/A";

        onCallback?.Invoke(name);
    }

    // 🔥 CHANGE NAME
    public void ChangeName()
    {
        userID = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

        if (string.IsNullOrEmpty(userID))
        {
            Debug.LogError("No hay usuario logueado");
            return;
        }

        db.Child("user")
          .Child(userID)
          .Child("name")
          .SetValueAsync(Usernameupdate.text);
    }

    // 🔥 DELETE USER (SOLO NAME)
    public void DeleteUserData()
    {
        userID = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

        if (string.IsNullOrEmpty(userID))
        {
            Debug.LogError("No hay usuario logueado");
            return;
        }

        db.Child("user")
          .Child(userID)
          .Child("name")
          .RemoveValueAsync();
    }

    // 🔥 UI LOAD
    public void Getuserinfo()
    {
        StartCoroutine(GetName((string name) =>
        {
            nametext.text = name;
        }));
    }
}