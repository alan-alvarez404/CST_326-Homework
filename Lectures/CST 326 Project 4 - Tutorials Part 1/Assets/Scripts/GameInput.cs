using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInput;
    
    private void Awake()
    {
        playerInput = new PlayerInputActions();
        playerInput.Player.Enable();
    }
    
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        
        inputVector = inputVector.normalized;
        return inputVector;
    }
    
    
}
