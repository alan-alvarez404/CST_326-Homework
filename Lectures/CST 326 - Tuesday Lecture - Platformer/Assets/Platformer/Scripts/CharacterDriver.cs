using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterDriver : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float groundAcceleration = 1f;

    public float airAcceleration = 0.5f;
    public float apexHeight = 4.5f;
    public float apexTime = 0.75f; // Default SHOULD BE 0.75
    
    CharacterController _controller;
    
    Animator _animator;

    float _velocityX;
    float _velocityY;   
    
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
                float jumpImpulse = 2f * apexHeight / apexTime;
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
            _velocityX += direction * airAcceleration * Time.deltaTime;
        }
        
        // Speed clamping
        float xMaxSpeed = runHeld ? runSpeed : walkSpeed;
        _velocityX = Mathf.Clamp(_velocityX,-xMaxSpeed, xMaxSpeed);
        
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
