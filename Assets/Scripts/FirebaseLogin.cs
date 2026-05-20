using UnityEngine;
using System;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;

public class FirebaseLogin  : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    // Optional automatic test: set in inspector (disabled by default)
    public bool autoTest = false;
    public string testEmail = "test@test.com";
    public string testPassword = "123456";

    void Start()
    {
        if (autoTest)
        {
            // Use Register then Login in the continuation to ensure the user exists
            Register(testEmail, testPassword, (ok, msg) =>
            {
                if (ok)
                    Login(testEmail, testPassword);
                else
                    Debug.LogWarning($"AutoTest Register failed: {msg}");
            });
        }
    }
    public bool FirebaseReady { get; private set; }

    private void Awake()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                DependencyStatus status = task.Result;

                if (status == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;
                    FirebaseReady = true;

                    Debug.Log("Firebase inicializado correctamente");
                }
                else
                {
                    Debug.LogError($"Error Firebase: {status}");
                }
            });
    }

    // =========================
    // REGISTER
    // =========================
    public void Register(string email, string password, Action<bool,string> callback = null)
    {
        if (!FirebaseReady)
        {
            Debug.LogError("Firebase no está listo");
            callback?.Invoke(false, "Firebase no está listo");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Registro cancelado");
                    callback?.Invoke(false, "Registro cancelado");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError($"Error registro: {task.Exception}");
                    callback?.Invoke(false, task.Exception.ToString());
                    return;
                }

                user = task.Result.User;

                Debug.Log($"Usuario creado: {user.Email}");
                callback?.Invoke(true, user.Email);
            });
    }

    // =========================
    // LOGIN
    // =========================
    public void Login(string email, string password, Action<bool,string> callback = null)
    {
        if (!FirebaseReady)
        {
            Debug.LogError("Firebase no está listo");
            callback?.Invoke(false, "Firebase no está listo");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Login cancelado");
                    callback?.Invoke(false, "Login cancelado");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError($"Error login: {task.Exception}");
                    callback?.Invoke(false, task.Exception.ToString());
                    return;
                }

                user = task.Result.User;

                Debug.Log($"Login correcto: {user.Email}");
                callback?.Invoke(true, user.Email);
            });
    }

    // =========================
    // LOGOUT
    // =========================
    public void Logout()
    {
        if (auth == null)
            return;

        auth.SignOut();

        user = null;

        Debug.Log("Sesión cerrada");
    }

    // =========================
    // GET CURRENT USER
    // =========================
    public FirebaseUser GetCurrentUser()
    {
        return auth != null ? auth.CurrentUser : null;
    }

    // =========================
    // CHECK LOGIN
    // =========================
    public bool IsLoggedIn()
    {
        return auth != null && auth.CurrentUser != null;
    }
}