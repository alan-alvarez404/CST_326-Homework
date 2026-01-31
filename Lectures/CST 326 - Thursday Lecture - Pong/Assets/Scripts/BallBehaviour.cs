using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class BallBehaviour : MonoBehaviour
{
    public float ballSpeed = 8f;
    private float angleFromStart = 90f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartBall();
        Debug.Log("Ball Started");
        Debug.Log("Velocity set to: " + rb.linearVelocity);
    }

    // Update is called once per frame
    void StartBall()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float direction =  Random.value < 0.5f ? -1f : 1f;
        float angle = Random.Range(-angleFromStart, angleFromStart) * Mathf.Deg2Rad;

        Vector3 dir = new Vector3(Mathf.Cos(angle) * direction, 0f, Mathf.Sin(angle)).normalized;

        rb.linearVelocity = dir * ballSpeed;
    }
}
