using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterDriver : MonoBehaviour
{
    [Header("Ground Movement Values")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float groundAcceleration = 1f;

    [Header("Air Movement Values")]
    public float airAcceleration = 15f; // Default SHOULD be 15f
    public float airDeceleration = 10f; // Needed for slowing the player down midair if they don't hold a movement key (default is 10f)
    public float apexHeight = 4.5f;
    public float apexTime = 0.7f; // Default SHOULD BE 0.7
    
    // These are needed for trying to make Mario fall at a sdet rate
    [Header("Miscellaneous")] 
    public static bool isForced = false;
    public static float forcedFallSpeed = 0f;
    
    CharacterController _controller;
    
    Animator _animator;

    float _velocityX;
    float _velocityY;   
    
    public float yMaxFallSpeed = -25f; // Needed for clamping the Y
    
    Quaternion _facingLeft;
    Quaternion _facingRight;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        
        _facingRight = Quaternion.Euler(0f, 90f, 0f);
        _facingLeft = Quaternion.Euler(0f, -90f, 0f);
        _animator = GetComponent<Animator>();
        _controller =  GetComponent<CharacterController>();
    }

    // Have to do this to be able to stop time in the next method properly
    private static CharacterDriver instance;
    
    public static void StopTheMario(bool stillFallling, float fallSpeed)
    {
        // Make it so mario's transform and momentum freezes

        instance._velocityX = 0f;
        instance._velocityY = 0f;
        
        // If stillFalling is true set these values used in Update()
        if (stillFallling)
        {
            isForced = true;
            forcedFallSpeed = fallSpeed;
            
            // Disable the character controller portion of this script
            instance._controller.enabled = false;
            return;
        }
        
        // Disable the entire movement scripting inside of Update
        instance.enabled = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Is checked immediately 
        if (isForced)
        {
            // Freeze the controls but make Mario fall at a set speed
            Vector3 position = transform.position;
            position.y += forcedFallSpeed * Time.deltaTime; 
            
            // Make Mario fall at the new velocity that is the forced fall speed
            transform.position = position;
            return;
        }
        
        
        float direction = 0f;
        if (Keyboard.current.dKey.isPressed) direction += 1f;
        if (Keyboard.current.aKey.isPressed) direction -= 1f;
        bool jumpPressedThisFrame = Keyboard.current.spaceKey.wasPressedThisFrame;
        bool jumpHeld = Keyboard.current.spaceKey.isPressed;
        bool runHeld = Keyboard.current.leftShiftKey.isPressed;
        
        if (_controller.isGrounded)
        {
            if (direction != 0)
            {
                _velocityX += direction * groundAcceleration * Time.deltaTime;
                transform.rotation = (direction > 0f) ? _facingRight : _facingLeft;
            }
            else
            {
                _velocityX = Mathf.MoveTowards(_velocityX, 0f, groundAcceleration * Time.deltaTime);
            }

            if (jumpPressedThisFrame)
            {
                float jumpImpulse = 2f * apexHeight / (apexTime);
                _velocityY = jumpImpulse;
            } else if (_velocityY < 0f)
            {
                _velocityY = -1f; // Stay grounded
            }
        }
        else // Air movement
        {
            float gravity = -2f * apexHeight / (apexTime * apexTime);
            
            if (!jumpHeld) gravity *= 2f;
            _velocityY += gravity * Time.deltaTime;
            
            // Calculation so that the player's momentum/direction can be changed midair rather than going in
            // the same direction if they gain speed and jump without being able to change it with movement keys.
            if (direction != 0f)
            {
                _velocityX += direction * airAcceleration * Time.deltaTime;
                transform.rotation = (direction > 0f) ? _facingRight : _facingLeft; // Change direction midair (don't know if old mario behaves like this)
            }
            else
            {
                _velocityX = Mathf.MoveTowards(_velocityX, 0f, airDeceleration * Time.deltaTime);
            }
        }
        
        // Speed clamping
        float xMaxSpeed = runHeld ? runSpeed : walkSpeed;
        _velocityX = Mathf.Clamp(_velocityX,-xMaxSpeed, xMaxSpeed);
        
        // Clamping the speed at which the player falls
        _velocityY = Mathf.Max(_velocityY, yMaxFallSpeed);
        
        Vector3 deltaPosition = new Vector3(_velocityX, _velocityY, 0f) * Time.deltaTime;
        
        CollisionFlags collisions = _controller.Move(deltaPosition);
        
        // Reset movement velocities based on object collisions
        if ((collisions & CollisionFlags.Above) != 0 && _velocityY > 0f)
        {
            BrickLogic.CheckForBrick(_controller, transform); // Callable method to check if whatever the player collides with is a brick
            CoinCountLogic.CheckForCB(_controller, transform); // Callable method to check if whatever the player collides with is a coin block

            _velocityY = 0f;
        }

        if ((collisions & CollisionFlags.Sides) != 0)
        {
            FlagpoleLogic.CheckForWin(_controller, transform); // Callable method to check if player hits flagpole
            _velocityX = 0f;
        }
        
        // Only check collisions from below without affecting the velocities (hopefully)
        if ((collisions & CollisionFlags.Below) != 0)
        {
            FlagpoleLogic.CheckForWin(_controller, transform); // Callable method to check if player hits flagpole
            WaterBlockLogic.CheckForWater(_controller, transform); // Callable method to check if player hits a dangerous water block

        }

        _animator.SetFloat("Speed", Mathf.Abs(_velocityX));
        _animator.SetBool("isGrounded", _controller.isGrounded);

        // Debug.Log($"Grounded: {_controller.isGrounded}"); // This runs every frame. Jesus Christ
    }
}
