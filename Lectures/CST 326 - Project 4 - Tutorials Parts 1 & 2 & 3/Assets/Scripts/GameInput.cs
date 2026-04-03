using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    
    public static GameInput Instance{ get; private set; }
    
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    public event EventHandler OnBindingRebind;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
    }
    
    private PlayerInputActions playerInput;
    
    private void Awake()
    {
        Instance = this;
        
        playerInput = new PlayerInputActions();
        
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInput.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        
        // At this point the tutorial discusses making controller navigation 
        // There is no way on god's green earth that I am going to play this specific
        // game with a controller just to test. I do have a controller (2 actually) but
        // when the time comes that I want to make a game have console support, then I
        // will gladfully come back to this bit of the tutorial.
        // Until then, skipping straight into the last bit which is polishing.
        
        playerInput.Player.Enable();

        playerInput.Player.Interact.performed += Interact_performed;
        playerInput.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInput.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInput.Player.Interact.performed -= Interact_performed;
        playerInput.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInput.Player.Pause.performed -= Pause_performed;

        playerInput.Dispose();
    }
    
    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (OnInteractAction != null)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (OnInteractAlternateAction != null)
        {
            OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInput.Player.Move.ReadValue<Vector2>();
        
        inputVector = inputVector.normalized;
        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                return playerInput.Player.Move.bindings[1].ToDisplayString();
            
            case Binding.Move_Down:
                return playerInput.Player.Move.bindings[2].ToDisplayString();
            
            case Binding.Move_Left:
                return playerInput.Player.Move.bindings[3].ToDisplayString();
            
            case Binding.Move_Right:
                return playerInput.Player.Move.bindings[4].ToDisplayString();
                
            case Binding.Interact:
                return playerInput.Player.Interact.bindings[0].ToDisplayString();
            
            case Binding.InteractAlternate:
                return playerInput.Player.InteractAlternate.bindings[0].ToDisplayString();
            
            case Binding.Pause:
                return playerInput.Player.Pause.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInput.Player.Disable();

        InputAction inputAction;
        int bindingIndex;
        
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = playerInput.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInput.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInput.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInput.Player.Move;
                bindingIndex = 4;
                break;
            
            case Binding.Interact:
                inputAction = playerInput.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerInput.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInput.Player.Pause;
                bindingIndex = 0;
                break;
        }
        
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
            {
                callback.Dispose();
                playerInput.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInput.SaveBindingOverridesAsJson()); // Save a JSON file 
                PlayerPrefs.Save();
                
                OnBindingRebind?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }
}
