using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PongPaddle : MonoBehaviour
{
    public float paddleSpeed = 1f;
    public float forcedStrength = 10f;

    public Key upKey = Key.W;
    public Key downKey = Key.S;

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

    // Update is called once per frame
    void Update()
    {
        // Wait to play the game or stop when game is over
        if (!GameState.CanPlay || GameState.GameOver)
        {
            return;
        }
        
        if (Keyboard.current[upKey].isPressed)
        {
            Vector3 force = new Vector3(0f, forcedStrength, 0f);
            
            // Moved the line that sets the rigidbody to the Awake method
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Force);
            }
        }

        if (Keyboard.current[downKey].isPressed)
        {
            Vector3 force = new Vector3(0f, -forcedStrength, 0f);
            
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Force);
            }
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
