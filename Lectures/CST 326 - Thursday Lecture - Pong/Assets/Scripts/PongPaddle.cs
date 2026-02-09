using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PongPaddle : MonoBehaviour
{
    public float paddleSpeed = 1f;
    public float forcedStrength = 10f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public Key upKey = Key.W;
    public Key downKey = Key.S;

    // For powerup scaling
    private Vector3 originalSize;
    private Coroutine sizeRoutine;

    private void Awake()
    {
        originalSize = transform.localScale; // Store the size upon starting
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
            
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(force, ForceMode.Force);
        }

        if (Keyboard.current[downKey].isPressed)
        {
            Vector3 force = new Vector3(0f, -forcedStrength, 0f);
            
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(force, ForceMode.Force);
        }

        float angle = 50f;
        
        Vector3 up = Vector3.up;
        Quaternion testRotation = Quaternion.Euler(60f, 0f, 0f);
        
        Vector3 rotatedVector = testRotation * up;

        Quaternion otherRotation = Quaternion.Euler(-60f, 0f, 0f);
        Vector3 otherRotatedVector = otherRotation * up;

        Quaternion someOtherAngleRotation = Quaternion.Euler(angle, 0f, 0f);
        Vector3 someOtherRotatedVector = someOtherAngleRotation * up;

        // Debug.DrawRay(transform.position, rotatedVector * 5f, Color.red);
        // Debug.DrawRay(transform.position, otherRotatedVector * 5f, Color.green);
        // Debug.DrawRay(transform.position, someOtherRotatedVector * 5f, Color.blue);
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
}
