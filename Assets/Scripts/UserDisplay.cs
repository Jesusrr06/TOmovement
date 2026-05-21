using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;

[Serializable]
public class UserDisplay : MonoBehaviour
{
    [Serializable]
    public class FieldMapping
    {
        [Tooltip("Ruta en la DB relativa al UID. Ej: Profile/name o Stats/LocalFights")]
        public string dbPath;

        [Tooltip("Nombre del GameObject en la jerarquía que contiene el componente Text o TMP_Text")]
        public string objectName;
    }

    public FirebaseLogin firebaseLogin; // opcional: obtener UID del usuario logeado
    public string databaseUrl = "https://tofight-be0d3-default-rtdb.europe-west1.firebasedatabase.app/";
    public string uid; // si está vacío, se intentará usar el usuario actual de firebaseLogin
    public FieldMapping[] mappings;

    [Header("Single display option")]
    [Tooltip("Si se asigna, muestra name y local fights en este Text/TMP. Si true, mappings serán ignorados.")]
    public TMP_Text displayTextTMP;
    public Text displayTextUI;
    public bool useSingleDisplay = false;

    private DatabaseReference db;

    void LoadBasicInfo(string uid)
    {
        db.Child(uid).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError("Error leyendo usuario: " + task.Exception);
                SetSingleDisplay("Error cargando datos");
                return;
            }
            var snap = task.Result;
            string name = (snap.Child("Profile").Child("name").Value != null) ? snap.Child("Profile").Child("name").Value.ToString() : "N/A";
            object fightsVal = snap.Child("Stats").Child("LocalFights").Value;
            long fights = 0;
            if (fightsVal != null)
            {
                try { fights = Convert.ToInt64(fightsVal); } catch { try { fights = (long)Convert.ToDouble(fightsVal); } catch { fights = 0; } }
            }
            SetSingleDisplay("Name: " + name + "\nFights: " + fights);
        });
    }

    void SetSingleDisplay(string text)
    {
        if (displayTextTMP != null) displayTextTMP.text = text;
        if (displayTextUI != null) displayTextUI.text = text;
    }

    void Start()
    {
        db = FirebaseDatabase.GetInstance(databaseUrl).RootReference;

        if (string.IsNullOrEmpty(uid) && firebaseLogin != null)
        {
            var user = firebaseLogin.GetCurrentUser();
            if (user != null) uid = user.UserId;
        }

        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogError("UID no proporcionado y no hay usuario logueado.");
            return;
        }

        if (mappings == null || mappings.Length == 0)
        {
            Debug.LogWarning("No hay mappings configurados en el inspector.");
            return;
        }

        foreach (var m in mappings)
        {
            LoadField(uid, m);
        }
    }

    void LoadField(string uid, FieldMapping mapping)
    {
        if (string.IsNullOrEmpty(mapping.dbPath) || string.IsNullOrEmpty(mapping.objectName))
        {
            Debug.LogWarning("Mapping inválido: " + JsonUtility.ToJson(mapping));
            return;
        }

        // Construir referencia a partir de dbPath (soporta rutas con '/').
        var parts = mapping.dbPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        DatabaseReference r = db.Child(uid);
        foreach (var p in parts) r = r.Child(p);

        r.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"Error leyendo '{mapping.dbPath}': {task.Exception}");
                return;
            }

            var snap = task.Result;
            string value = (snap != null && snap.Value != null) ? snap.Value.ToString() : string.Empty;

            var go = GameObject.Find(mapping.objectName);
            if (go == null)
            {
                Debug.LogWarning($"No se encontró GameObject '{mapping.objectName}' para la ruta '{mapping.dbPath}'");
                return;
            }

            var tmp = go.GetComponent<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = value;
                return;
            }

            var uiText = go.GetComponent<Text>();
            if (uiText != null)
            {
                uiText.text = value;
                return;
            }

            Debug.LogWarning($"El GameObject '{mapping.objectName}' no tiene componente TMP_Text ni Text.");
        });
    }
}

