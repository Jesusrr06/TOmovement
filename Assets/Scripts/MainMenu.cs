using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
    Debug.Log("BOTON FUNCIONA");

        SceneManager.LoadScene("main");
    }

    public void  QuitGame()
    {
    Debug.Log("BOTON FUNCIONA");

        Application.Quit();
    }
}