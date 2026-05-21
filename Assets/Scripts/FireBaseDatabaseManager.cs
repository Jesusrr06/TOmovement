using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor; // Required for editor-specific settings if testing in editor
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
    DatabaseReference reference;

    void Start()
    {
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Set up the Editor-specific configuration for the Realtime Database.
                // This is important if you're testing directly in the Unity editor.
                // Replace "YOUR_DATABASE_URL" with your actual Realtime Database URL (e.g., https://your-project-id-default-rtdb.firebaseio.com)
                // You can find this in your Firebase Console under Realtime Database -> Data tab.
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://tofight-be0d3-default-rtdb.europe-west1.firebasedatabase.app");

                // Get the root reference location of the database.
                reference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Realtime Database initialized successfully!");

                // Example usage:
                WriteNewUser("player123", "John Doe", "john@example.com");
                ListenForDataChanges();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    // --- Writing Data ---
    public void WriteNewUser(string userId, string name, string email)
    {
        User newUser = new User(name, email);
        string json = JsonUtility.ToJson(newUser);

        // This will create a child under "users" with the userId as its key
        // and store the user data.
        reference.Child("users").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWith(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("User data saved successfully!");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError($"Failed to save user data: {task.Exception}");
                }
            });
    }

    // A simple class to represent your user data structure
    [System.Serializable]
    public class User
    {
        public string username;
        public string email;

        public User(string username, string email)
        {
            this.username = username;
            this.email = email;
        }
    }

    // --- Reading Data ---
    public void ReadUserData(string userId)
    {
        reference.Child("users").Child(userId).GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // Convert the JSON data back to our User object
                    string json = snapshot.GetRawJsonValue();
                    User user = JsonUtility.FromJson<User>(json);
                    Debug.Log($"Read user: {user.username}, Email: {user.email}");
                }
                else
                {
                    Debug.LogWarning($"User with ID {userId} not found.");
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError($"Failed to read user data: {task.Exception}");
            }
        });
    }

    // --- Listening for Real-time Changes ---
    // This method sets up a listener that will be triggered whenever data under "users" changes.
    public void ListenForDataChanges()
    {
        reference.Child("users").ValueChanged += HandleValueChanged;
        Debug.Log("Listening for user data changes...");
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        if (snapshot.Exists)
        {
            // Iterate through all users if multiple users are returned
            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                string json = childSnapshot.GetRawJsonValue();
                User user = JsonUtility.FromJson<User>(json);
                Debug.Log($"Real-time Update - User ID: {childSnapshot.Key}, Name: {user.username}, Email: {user.email}");
            }
        }
        else
        {
            Debug.Log("Real-time Update - No user data found at this path.");
        }
    }

    // Important: Remove the listener when the GameObject is destroyed to prevent memory leaks.
    void OnDestroy()
    {
        if (reference != null)
        {
            reference.Child("users").ValueChanged -= HandleValueChanged;
        }
    }
}
