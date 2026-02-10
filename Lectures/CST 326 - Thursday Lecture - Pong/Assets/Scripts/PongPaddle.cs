using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PongPaddle : MonoBehaviour
{
    public float paddleSpeed = 1f;
    public float forcedStrength = 10f;

    // Replaced the up and down keys to utilize inputActions
    public InputActionReference moveAction; 

    // For powerup scaling
    private Vector3 originalSize;
    private Coroutine sizeRoutine;

    // Needed for resetting the paddle positions
    private Vector3 startPos;
    private Quaternion startRot;
    private Rigidbody rb;
    
    private void Awake()
    {
        originalSize = transform.localScale; // Share the size upon starting

        startPos = transform.position;
        startRot = transform.rotation;

        rb = GetComponent<Rigidbody>();
    }
    
    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Wait to play the game or stop when game is over
        if (!GameState.CanPlay || GameState.GameOver)
        {
            return;
        }

        // In case the moveAction was never assigned        
        if (moveAction == null)
        {
            return;
        }

        
        // This section utilizes inputActions
        float move = moveAction.action.ReadValue<float>(); // -1 to +1

        if (move > 0.1f)
        {
            Vector3 force = new Vector3(0f, forcedStrength, 0f);
            rb.AddForce(force, ForceMode.Force);
        }
        else if (move < -0.1f)
        {
            Vector3 force = new Vector3(0f, -forcedStrength, 0f);
            rb.AddForce(force, ForceMode.Force);
        }
    }
    
    // Make the paddles bigger when collecting a powerup
    public void makePaddlesBigger(float scaleMultiplier, float durationSeconds)
    {
        if (sizeRoutine != null)
        {
            StopCoroutine(sizeRoutine);
        }
        
        sizeRoutine = StartCoroutine(paddleSizeRoutine(scaleMultiplier, durationSeconds));
    }
    
    private IEnumerator paddleSizeRoutine(float scaleMultiplier, float durationSeconds)
    {
        transform.localScale = originalSize * scaleMultiplier;
        
        yield return new WaitForSeconds(durationSeconds); // Pause the function
        
        // Resume the function
        transform.localScale = originalSize;
        sizeRoutine = null;
    }
    
    // Will be called by LoadGame.cs when a player wins
    public void resetAndFreezePosition()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        // Also reset the size of the paddles in case a poowerup is active
        // the moment that a player paddle wins
        transform.localScale = originalSize;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
