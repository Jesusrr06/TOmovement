using UnityEngine;
using Firebase.Auth;
using Firebase.Database;

public class PlayerDataRtdb : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference db;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // =========================
    // 📥 CARGAR SOLO PROFILE
    // =========================
    public void LoadProfile()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("No hay usuario logueado");
            return;
        }

        string uid = auth.CurrentUser.UserId;

        db.Child(uid).Child("Profile").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    string name = snapshot.Child("name").Value.ToString();
                    Debug.Log("Player name: " + name);
                }
                else
                {
                    Debug.Log("No existe Profile");
                }
            }
        });
    }

    // =========================
    // 📊 CARGAR SOLO STATS
    // =========================
    public void LoadStats()
    {
        if (auth.CurrentUser == null)
        {
            Debug.LogError("No hay usuario logueado");
            return;
        }

        string uid = auth.CurrentUser.UserId;

        db.Child(uid).Child("Stats").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    long localFights = (long)snapshot.Child("LocalFights").Value;
                    Debug.Log("LocalFights: " + localFights);
                }
                else
                {
                    Debug.Log("No existe Stats");
                }
            }
        });
    }
}