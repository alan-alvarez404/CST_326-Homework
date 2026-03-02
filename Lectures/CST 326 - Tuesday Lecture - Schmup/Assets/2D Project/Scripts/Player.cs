using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootOffsetTransform;
    
    // Doing this similarly to the platformer to clamp speed
    public float moveSpeed = 10f; // 10 is good
    public float moveAcceleration = 5f; // 5 is good
    public float decelerationMultiplier = 8f; // 8 is good
    
    CharacterController controller;
    
    float _velocityX;
    float _velocityY;   
    
    private Vector3 startingPosition;
    
    // private AudioController audioController;
    
    void Start()
    {
        // This is where I would cache and get the animator but the player tank literally onely has one sprite
        
        _instance = this;
        
        startingPosition = transform.position;
        controller = GetComponent<CharacterController>();

        // audioController = audioController.Instance;
    }

    // Needed for the next function
    private static Player _instance;
    
    // Will be called by other scripts to stop the tank from moving
    public static void stopThatTank()
    {
        if (_instance == null) return;
        
        _instance._velocityX = 0f;
        _instance._velocityY = 0f;
        
        // Entirely disable movement
        _instance.enabled = false;
    }

    void OnDestroy()
    {
        if(_instance == this) _instance = null;
    }
    
    // Gonna do movement similar to the Nario platformer to clamp speed
    void Update()
    {
        float direction = 0f;
        if (Keyboard.current.rightArrowKey.isPressed) direction += 1f;
        if (Keyboard.current.leftArrowKey.isPressed) direction -= 1f;

        if (direction != 0f)
        {
            _velocityX += direction * moveAcceleration * Time.deltaTime;
        }
        else
        {
            _velocityX = Mathf.MoveTowards(_velocityX, 0f, (moveAcceleration * decelerationMultiplier) * Time.deltaTime); // Slow em down
        }
        
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameObject shot = Instantiate(bulletPrefab, shootOffsetTransform.position, Quaternion.identity);
            Debug.Log("Bang!");

            // todo - destroy the bullet after 3 seconds
            Destroy(shot, 3f); // Overloaded to wait 3 seconds

            if (GetComponent<Animator>() != null)
            {
                // this is where I would trigger shoot animation but once again the player tank has no shooting sprites
                GetComponent<Animator>().SetTrigger("Shot Trigger");
            }
        }

        // Speed clamping
        float xMaxSpeed = moveSpeed;
        _velocityX = Mathf.Clamp(_velocityX, -xMaxSpeed, xMaxSpeed);
        
        Vector3 deltaPosition = new Vector3(_velocityX, _velocityY, 0f) * Time.deltaTime;

        transform.position += deltaPosition;
        ClampPlayerX();
    }

    // Reusing the code for determining left and right canvas edges via ViewportToWorldPoint
    void ClampPlayerX()
    {
        // Getting the camera for calculations
        Camera main = Camera.main;
        float distanceForZ = -main.transform.position.z; // Currently the camera's z in the inspector is -1, make it positive for future calculations
        
        // Left and Right Edges
        float leftEdge = main.ViewportToWorldPoint(new Vector3(0f, 0f, distanceForZ)).x;
        float rightEdge = main.ViewportToWorldPoint(new Vector3(1f, 0f, distanceForZ)).x;

        float halfWidth = 0f;
        
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            halfWidth = spriteRenderer.bounds.extents.x;
        }

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, leftEdge + halfWidth, rightEdge - halfWidth);
        transform.position = position;
    }
}
