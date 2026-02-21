using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterDriver : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float groundAcceleration = 1f;

    public float airAcceleration = 15f; // 0.5 might be too small, switch to 1.0
    public float airDeceleration = 10f; // Needed for slowing the player down mid air if they dont hold a movement key(?)
    public float apexHeight = 4.5f;
    public float apexTime = 0.7f; // Default SHOULD BE 0.7
    
    CharacterController _controller;
    
    Animator _animator;

    float _velocityX;
    float _velocityY;   
    
    public float yMaxFallSpeed = -25f; // Needed for clamping the Y
    
    Quaternion facingLeft;
    Quaternion facingRight;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        facingRight = Quaternion.Euler(0f, 90f, 0f);
        facingLeft = Quaternion.Euler(0f, -90f, 0f);
        _animator = GetComponent<Animator>();
        _controller =  GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
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
                transform.rotation = (direction > 0f) ? facingRight : facingLeft;
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
            //_velocityX += direction * airAcceleration * Time.deltaTime;
            
            // Calculation so that the player's momentum/direction can be changed mid air rather than going in
            // the same direction if they gain speed and jump without being able to change it with movement keys.
            if (direction != 0f)
            {
                _velocityX += direction * airAcceleration * Time.deltaTime;
                transform.rotation = (direction > 0f) ? facingRight : facingLeft; // Change direction mid air (don't know if old mario behaves like this)
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
        if ((collisions & CollisionFlags.Above) != 0 && _velocityY > 0f) _velocityY = 0f;
        if ((collisions & CollisionFlags.Sides) != 0) _velocityX = 0f;

        _animator.SetFloat("Speed", Mathf.Abs(_velocityX));
        _animator.SetBool("isGrounded", _controller.isGrounded);

        // Debug.Log($"Grounded: {_controller.isGrounded}"); // This runs every frame. Jesus Christ
    }
}
