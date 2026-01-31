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

    

    // Update is called once per frame
    void Update()
    {
        
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

        Debug.DrawRay(transform.position, rotatedVector * 5f, Color.red);
        Debug.DrawRay(transform.position, otherRotatedVector * 5f, Color.green);
        Debug.DrawRay(transform.position, someOtherRotatedVector * 5f, Color.blue);
    }
}
