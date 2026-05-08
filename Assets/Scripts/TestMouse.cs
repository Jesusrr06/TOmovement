using UnityEngine;

public class TestMouse : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse funcionando");
        }
    }
}