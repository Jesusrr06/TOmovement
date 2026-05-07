using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MovementInputVector { get; private set; }
    public event Action OnJumpButtonPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnMove(InputValue inputValue)
    {
        
        MovementInputVector = inputValue.Get<Vector2>();
    }

    private void OnJump(InputValue inputValue)
    { 
        if(inputValue.isPressed)
        {      

            OnJumpButtonPressed?.Invoke();
        }
        
    }

    private void Attack()
    {
    }

}
