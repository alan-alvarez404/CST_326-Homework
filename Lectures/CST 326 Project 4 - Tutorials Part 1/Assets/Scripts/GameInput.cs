using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    
    private PlayerInputActions playerInput;
    
    private void Awake()
    {
        playerInput = new PlayerInputActions();
        playerInput.Player.Enable();

        playerInput.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (OnInteractAction != null)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        
        inputVector = inputVector.normalized;
        return inputVector;
    }
    
    
}
