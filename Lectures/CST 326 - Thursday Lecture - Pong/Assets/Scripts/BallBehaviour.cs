using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public float ballSpeed = 8f;
    public float speedIncreaseOnBounce = 5f;
    public float bounceAngle = 60f;
    public float maxAngle = 35f;
    public float minimumYAngle = 0.20f; // Used to properly bounce off top and bottom walls

    // For whenever a new ball from the powerup spawns
    public bool isBonus = false;
    public int bonusPoints = 3;
    
    private bool gameStarted;
    
    private Rigidbody rb;
    private float currentSpeed;

    public Material bonusBallMaterial;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = ballSpeed;
        // Keep the ball frozen 
        rb.linearVelocity = Vector3.zero;

        // StartBall();
        // Debug.Log("Ball Started");
        // Debug.Log("Velocity set to: " + rb.linearVelocity);
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
        dir = fixMinYReflection(dir);
        
        rb.linearVelocity = dir * currentSpeed;
    }

    private void OnCollisionEnter(Collision c)
    {
        float speed = rb.linearVelocity.magnitude; // Used for pitch calculations relating to audio
        
        // If the current tag is the player's paddle tag
        if (c.gameObject.CompareTag("Paddle"))
        {

            // Speed increase upon rebounce
            currentSpeed += speedIncreaseOnBounce;

            // Changed the variables to use c.collider and c.contacts to that if we ever
            // Decide to have the paddles change in some way (via powerups which will happen)
            float ballContactY = c.contacts[0].point.y; // This is where the ball hit the paddle

            float paddleCenterY = c.collider.bounds.center.y; // Center of the paddle
            
            float halfPaddle = c.collider.bounds.extents.y; // Simply half the paddle

            float t = 0f;
            if (halfPaddle > 0.0001f)
            {
                t = (ballContactY - paddleCenterY) / halfPaddle;
                t = Mathf.Clamp(t, -1f, 1f);
            }

            float angleRad = t * bounceAngle * Mathf.Deg2Rad;

            float direction = -1f;
            if (transform.position.z > c.transform.position.z)
            {
                direction = 1f;
            }

            Vector3 dir = new Vector3(0f, Mathf.Sin(angleRad), Mathf.Cos(angleRad) * direction).normalized;
            dir = fixMinYReflection(dir);

            rb.linearVelocity = dir * currentSpeed;
            
            
            // Play the audio
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayPaddleHit(speed);
            }
            return;
        }

        // If the current tag is a top or bottom wall
        if (c.gameObject.CompareTag("TopAndBottomWalls"))
        {
            Vector3 v = rb.linearVelocity;
            Vector3 n = c.contacts[0].normal;
            Vector3 reflected = Vector3.Reflect(v, n);
            
            // This was the old logic for trying to reflect the ball off the top and bottom walls
            // It sucks
            /*
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
            */
            
            Vector3 dir = reflected.normalized;
            dir = fixMinYReflection(dir);
            
            rb.linearVelocity = dir * currentSpeed;
            Debug.Log("Hit: " + c.gameObject.name + " tag=" + c.gameObject.tag);
            
            // Play the audio
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayWallHit(speed);
            }
            
            return;
        }
    }
    
    // Added to fix the ball's velocity randomly decreasing upon bounce
    private void FixedUpdate()
    {
        if (!GameState.CanPlay)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        if (!gameStarted)
        {
            StartBall();
            gameStarted = true;
        }

        
        
        if (rb.linearVelocity.sqrMagnitude > 0.0001f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }
        
        //Debug.Log("Speed magnitude: " + rb.linearVelocity.magnitude + " | vel: " + rb.linearVelocity);
    }
    
    // This is the new function to stabiilize and fix the ball's Y reflected direction
    private Vector3 fixMinYReflection(Vector3 dir)
    {
        float absY = Mathf.Abs(dir.y);
        if (absY >= minimumYAngle)
        {
            return dir;
        }

        float signY = 1f;
        if (dir.y < 0f)
        {
            signY = -1f;
        }
        else if (dir.y > 0f)
        {
            signY = 1f;
        }
        else
        {
            if (Random.value < 0.5f)
            {
                signY = -1f;
            }
        }

        float signZ = 1f;
        if (dir.z < 0f)
        {
            signZ = -1f;
        }
        else if (dir.z > 0f)
        {
            signZ = 1f;
        }
        else
        {
            if (Random.value < 0.5f)
            {
                signZ = -1f;
            }
        }

        float y = minimumYAngle * signY;
        float z = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y)) * signZ;

        return new Vector3(0f, y, z);
    }
    
    
    // Following is for the powerup ball behavior specifically
    public void changeSizeAndSpeed(float sizeMultiplier, float speedMultiplier)
    {
        transform.localScale *= sizeMultiplier;
        
        currentSpeed *= speedMultiplier;

        if (rb.linearVelocity.sqrMagnitude > 0.0001f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }
    }
    
    // Following is for the powerup ball behavior specifically
    public void spawnBonusBall()
    {
        Vector3 spawnPos = transform.position;

        LoadGame ldg = FindFirstObjectByType<LoadGame>();
        if (ldg != null && ldg.ballSpawn != null)
        {
            spawnPos = ldg.ballSpawn.position;
        }

        GameObject newBall = Instantiate(gameObject, spawnPos, Quaternion.identity);

        BallBehaviour ballBehaviour = newBall.GetComponent<BallBehaviour>();
        if (ballBehaviour != null)
        {
            ballBehaviour.isBonus = true;
            ballBehaviour.bonusPoints = 3;

            ballBehaviour.currentSpeed = ballBehaviour.ballSpeed;
            ballBehaviour.StartBall();
        }
        
        // Specifically a different material for the bonus ball
        Renderer r = newBall.GetComponentInChildren<Renderer>();
        if (r != null && bonusBallMaterial != null)
        {
            r.material = bonusBallMaterial;
        }
    }
}
