using UnityEngine;

/// <summary>
/// Simple debug helper: logs mouse clicks to the console during play.
/// </summary>
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