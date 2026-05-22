using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public Image[] player1Options;
    public Image[] player2Options;

    public Image selectorP1;
    public Image selectorP2;
   private int _indexP1=0 ;
   private int _indexP2=0 ;

    void Start()
    {
        UpdateSelectors();
    }

    void Update()
    {
        // PLAYER 1 (WASD)
        if (Input.GetKeyDown(KeyCode.D))
        {
            _indexP1++;
            if (_indexP1 >= player1Options.Length) _indexP1 = 0;
            UpdateSelectors();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _indexP1--;
            if (_indexP1 < 0) _indexP1 = player1Options.Length - 1;
            UpdateSelectors();
        }

        // PLAYER 2 (ARROWS)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _indexP2++;
            if (_indexP2 >= player2Options.Length) _indexP2 = 0;
            UpdateSelectors();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _indexP2--;
            if (_indexP2 < 0) _indexP2 = player2Options.Length - 1;
            UpdateSelectors();
        }

        // CONFIRMAR
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            GameData.Player1Character = _indexP1;
            Debug.Log("P1 eligió: " + player1Options[_indexP1].name);
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {

            GameData.Player2Character = _indexP2;
            Debug.Log("P2 eligió: " + player2Options[_indexP2].name);

        }
    }

    void UpdateSelectors()
    {    Vector3 offset = new Vector3(0, -80f, 0); // ajusta este valor

        selectorP1.transform.position = player1Options[_indexP1].transform.position+offset;
        selectorP2.transform.position = player2Options[_indexP2].transform.position+offset;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }
    
}