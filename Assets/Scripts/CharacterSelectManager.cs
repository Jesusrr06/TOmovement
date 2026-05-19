using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public int selectedP1;
    public int selectedP2;

    public void SelectP1(int index)
    {
        selectedP1 = index;
        GameData.Player1Character = index;
    }

    public void SelectP2(int index)
    {
        selectedP2 = index;
        GameData.Player2Character = index;
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }
}