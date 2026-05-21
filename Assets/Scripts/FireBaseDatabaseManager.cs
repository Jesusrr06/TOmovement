using System;
using System.Collections;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
public TMP_InputField Username;
public  TMP_InputField Password;
    private String userID;
    private const string DATABASE_URL =
        "https://tofight-be0d3-default-rtdb.europe-west1.firebasedatabase.app/";
    private DatabaseReference db;

    public TMP_Text nametext;
    public TMP_Text fightstext;
   void Start()
    {
        // Conectar a Firebase Realtime Database usando URL manual
        FirebaseDatabase database = FirebaseDatabase.GetInstance(DATABASE_URL);

        // Referencia raíz
        db = database.RootReference;

        Debug.Log("✅ Firebase Database conectada correctamente");
    }
    public void CreateUser()
    {
        UserData newUser= new UserData(Username.text,int.Parse(Password.text));
       String json= JsonUtility.ToJson(newUser);
       db.Child("users").Child(userID).SetRawJsonValueAsync(json);
    }

    public IEnumerator GetName(Action<string> onCallback)
    {
        var usernamddata = db.Child("user").Child(userID).Child("name").GetValueAsync();

        yield return new WaitUntil(predicate: () => usernamddata.IsCompleted);
        if (usernamddata != null)
        {
            DataSnapshot snapshot = usernamddata.Result;

            onCallback.Invoke(snapshot.Value.ToString());

        }
    }


    public IEnumerator GetFight(Action<int> onCallback)
    {
        var usernamddata = db.Child("user").Child(userID).Child("Stats").GetValueAsync();

       yield return new WaitUntil(predicate:() => usernamddata.IsCompleted);
       if (usernamddata != null)
       {
           DataSnapshot snapshot = usernamddata.Result;
              
           onCallback.Invoke((int)snapshot.Value);
              
       }
    }

    public void Getuserinfo()
    {
        
        StartCoroutine(GetName((String name) =>
            {
                nametext.text = name;

            }));
        StartCoroutine(GetFight((int fights) =>
        {
            fightstext.text = "Fights" +fights ;

        }));

    }
    
} 