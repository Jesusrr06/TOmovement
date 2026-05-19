using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public Image[] player1Options;
    public Image[] player2Options;

    public Image selectorP1;
    public Image selectorP2;

    int indexP1 = 0;
    int indexP2 = 0;

    void Start()
    {
        UpdateSelectors();
    }

    void Update()
    {
        // PLAYER 1 (WASD)
        if (Input.GetKeyDown(KeyCode.D))
        {
            indexP1++;
            if (indexP1 >= player1Options.Length) indexP1 = 0;
            UpdateSelectors();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            indexP1--;
            if (indexP1 < 0) indexP1 = player1Options.Length - 1;
            UpdateSelectors();
        }

        // PLAYER 2 (ARROWS)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            indexP2++;
            if (indexP2 >= player2Options.Length) indexP2 = 0;
            UpdateSelectors();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            indexP2--;
            if (indexP2 < 0) indexP2 = player2Options.Length - 1;
            UpdateSelectors();
        }

        // CONFIRMAR
        if (Input.GetKeyDown(KeyCode.Return))
           GameData.Player1Character= indexP1;
        Debug.Log("P1 eligió: " + player1Options[indexP1].name);

        if (Input.GetKeyDown(KeyCode.RightShift))
            GameData.Player2Character = indexP2;
        Debug.Log("P2 eligió: " + player2Options[indexP2].name);
    }

    void UpdateSelectors()
    {    Vector3 offset = new Vector3(0, -80f, 0); // ajusta este valor

        selectorP1.transform.position = player1Options[indexP1].transform.position+offset;
        selectorP2.transform.position = player2Options[indexP2].transform.position+offset;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }
    
}