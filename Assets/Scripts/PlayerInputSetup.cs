using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSetup : MonoBehaviour
{
    void Awake()
    {
        // Check if PlayerInput exists
        PlayerInput playerInput = GetComponent<PlayerInput>();
        
        if (playerInput == null)
        {
            Debug.Log("Creating PlayerInput component...");
            playerInput = gameObject.AddComponent<PlayerInput>();
        }

        // Load the input actions asset
        InputActionAsset inputActions = Resources.Load<InputActionAsset>("InputSystem_Actions");
        if (inputActions != null)
        {
            playerInput.actions = inputActions;
            Debug.Log("✓ PlayerInput setup complete!");
        }
        else
        {
            Debug.LogError("Could not find InputSystem_Actions.inputactions file!");
        }

        // Self-destruct after setup
        Destroy(this);
    }
}
