using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseLogin : MonoBehaviour
{
    public bool FirebaseReady { get; private set; }

    private FirebaseAuth auth;

    void Awake()
    {
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseReady = false;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;

            if (status == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                FirebaseReady = true;

                Debug.Log("✅ Firebase inicializado correctamente");
            }
            else
            {
                FirebaseReady = false;
                Debug.LogError("❌ Firebase no disponible: " + status);
            }
        });
    }

    public void Register(string email, string password, Action<bool, string> callback)
    {
        if (!FirebaseReady)
        {
            callback(false, "Firebase no está listo");
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    string msg = GetError(task.Exception);
                    callback(false, msg);
                    return;
                }

                if (task.IsCanceled)
                {
                    callback(false, "Registro cancelado");
                    return;
                }

                callback(true, task.Result.User.Email);
            });
    }

    public void Login(string email, string password, Action<bool, string> callback)
    {
        if (!FirebaseReady)
        {
            callback(false, "Firebase no está listo");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    string msg = GetError(task.Exception);
                    callback(false, msg);
                    return;
                }

                if (task.IsCanceled)
                {
                    callback(false, "Login cancelado");
                    return;
                }

                callback(true, task.Result.User.Email);
            });
    }

    public void Logout()
    {
        if (auth != null)
            auth.SignOut();
    }
    public FirebaseUser GetCurrentUser()
    {
        return auth != null ? auth.CurrentUser : null;
    }

    private string GetError(Exception exception)
    {
        if (exception == null)
            return "Error desconocido";

        FirebaseException firebaseEx =
            exception.GetBaseException() as FirebaseException;

        if (firebaseEx != null)
        {
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    return "Esa cuenta ya existe";

                case AuthError.InvalidEmail:
                    return "Correo inválido";

                case AuthError.WeakPassword:
                    return "La contraseña es muy débil";

                case AuthError.WrongPassword:
                    return "Contraseña incorrecta";

                case AuthError.UserNotFound:
                    return "No existe una cuenta con ese correo";

                case AuthError.MissingPassword:
                    return "Falta la contraseña";

                case AuthError.MissingEmail:
                    return "Falta el correo";

                default:
                    return "Firebase error: " + errorCode;
            }
        }

        return exception.Message;
    }
}