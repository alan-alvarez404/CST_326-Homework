using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public float ballSpeed = 8f;
    public float speedIncreaseOnBounce = 0f;
    private float bounceAngle = 60f;
    public float maxAngle = 35f;
    public float minimumYAngle = 0.15f;


    private Rigidbody rb;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = ballSpeed;
        StartBall();
        Debug.Log("Ball Started");
        Debug.Log("Velocity set to: " + rb.linearVelocity);
    }
    
    // For resetting the ball
    public void resetBall(Vector3 ballSpawn)
    {
        currentSpeed = ballSpeed;

        transform.position = ballSpawn;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        StartBall();
    }

    public void StartBall()
    {
        float direction = 1f;
        if (Random.value < 0.5f)
        {
            direction = -1f;
        }
        
        // Ball will start traveling in a random angle upon game staert
        float angleRad = Random.Range(-maxAngle, maxAngle) * Mathf.Deg2Rad;
        
        Vector3 dir = new Vector3(0f, Mathf.Sin(angleRad), Mathf.Cos(angleRad) * direction).normalized;
        
        rb.linearVelocity = dir * currentSpeed;
    }

    private void OnCollisionEnter(Collision c)
    {
        // If the current tag is the player's paddle tag
        if (c.gameObject.CompareTag("Paddle"))
        {

            // Speed increase upon rebounce
            currentSpeed += speedIncreaseOnBounce;

            // Detect Z axis
            float yOffset = transform.position.y - c.transform.position.y;

            float halfPaddle = c.collider.bounds.extents.y;

            float t = 0f;
            if (halfPaddle > 0f)
            {
                t = yOffset / halfPaddle;
                t = Mathf.Clamp(t, -1f, 1f);
            }

            float angleRad = t * bounceAngle * Mathf.Deg2Rad;

            float direction = -1f;
            if (transform.position.z > c.transform.position.z)
            {
                direction = 1f;
            }

            Vector3 dir = new Vector3(0f, Mathf.Sin(angleRad), Mathf.Cos(angleRad) * direction).normalized;
            rb.linearVelocity = dir * currentSpeed;
            return;
        }

        // If the current tag is a top or bottom wall
        if (c.gameObject.CompareTag("TopAndBottomWalls"))
        {
            Vector3 v = rb.linearVelocity;
            Vector3 n = c.contacts[0].normal;
            Vector3 reflected = Vector3.Reflect(v, n);
            
            // Check so that when the ball hits the wall it bounces back in an appropiate angle
            // Before it would appear as if sliding along these walls
            if (Mathf.Abs(reflected.y) < minimumYAngle)
            {
                float sign = 1f;
                
                if (reflected.y != 0f)
                {
                    sign = Mathf.Sign(reflected.y);
                } else if (n.y != 0f)
                {
                    sign = Mathf.Sign(n.y);
                }

                reflected.y = sign * minimumYAngle;
            }
            
            rb.linearVelocity = reflected.normalized * currentSpeed;
            Debug.Log("Hit: " + c.gameObject.name + " tag=" + c.gameObject.tag);
            return;
        }
    }
    
    // Added to fix the ball's velocity randomly decreasing upon bounce
    private void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.0001f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }
        
        //Debug.Log("Speed magnitude: " + rb.linearVelocity.magnitude + " | vel: " + rb.linearVelocity);
    }

    
    // Old function (unused)
    /*
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
    */
}
