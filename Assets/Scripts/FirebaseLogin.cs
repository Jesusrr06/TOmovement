using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;

public class FirebaseLogin  : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    public FirebaseLogin firebaseLogin;

    void Start()
    {
        firebaseLogin.Register("test@test.com", "123456");
    
        firebaseLogin.Login("test@test.com", "123456");
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
    public void Register(string email, string password)
    {
        if (!FirebaseReady)
        {
            Debug.LogError("Firebase no está listo");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Registro cancelado");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError($"Error registro: {task.Exception}");
                    return;
                }

                user = task.Result.User;

                Debug.Log($"Usuario creado: {user.Email}");
            });
    }

    // =========================
    // LOGIN
    // =========================
    public void Login(string email, string password)
    {
        if (!FirebaseReady)
        {
            Debug.LogError("Firebase no está listo");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Login cancelado");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError($"Error login: {task.Exception}");
                    return;
                }

                user = task.Result.User;

                Debug.Log($"Login correcto: {user.Email}");
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
        return auth.CurrentUser;
    }

    // =========================
    // CHECK LOGIN
    // =========================
    public bool IsLoggedIn()
    {
        return auth.CurrentUser != null;
    }
}