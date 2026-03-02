using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    public Transform shootOffsetTransform;
    
    public AudioClip enemyTic;
    public AudioClip enemyTac;

    public int enemyType;

    public static float wallStepDetectionDistance = 0.25f; // Shared within the enemy movement script
    
    public delegate void EnemyDiedFunc(float points); // Func is delegate type
    public static event EnemyDiedFunc OnEnemyDied;
    
    // Making a new delegate and event for when an enemy reaches the rightmost or leftmost sides of the game
    public delegate void EnemyTouchedWallFunc(int whichWall);
    public static event EnemyTouchedWallFunc OnEnemyTouchBorder;
    
    private bool hitWall = false;
    
    // Making a new delegate and event for when an enemy reaches the area where the player borders spawn
    public delegate void EnemyCrossedBorderFunc(bool crossed);
    public static event EnemyCrossedBorderFunc OnEnemyCrossBorder;
    
    // Static variables are associated with the object
    // It's associated with a class definition and not an instance (these are notes btw)

    void Update()
    {
        CheckForWall(); // Check for left or right wall every frame
    }

    void Awake()
    {
        // Doing this here manually so that each enemy finds the right shooting offset transform
        // I tried doing this in the Unity inspector but I couldn't get it to work
        if (shootOffsetTransform == null)
        {
            // Look for the transform called Shooting Offset
            Transform t = transform.Find("Shooting Offset");
            if (t != null) shootOffsetTransform = t;
        }
    }
    
    void OnEnable()
    {
        EnemyShmovement.OnEnemyStep += TryToShoot;
    }

    void OnDisable()
    {
        EnemyShmovement.OnEnemyStep -= TryToShoot;
    }

    void TryToShoot()
    {
        int randValue = Random.Range(1, 11); // Generate random num b

        if (randValue == 1)
        {
            // Reweriting this so that it fires the bullet and returns a bool when its fired
            bool fired = Bullet.ShootBullet(enemyBulletPrefab, shootOffsetTransform.position, true);
            if (fired)
            {
                Debug.Log("Enemy Fired!");
            }
        }
        
    }

    void CheckForWall()
    {
        // Getting the camera for calculations
        Camera main = Camera.main;
        float distanceForZ = -main.transform.position.z; // Currently the camera's z in the inspector is -1, make it positive for future calculations
        
        // Left and Right Edges
        float leftEdge = main.ViewportToWorldPoint(new Vector3(0f, 0f, distanceForZ)).x;
        float rightEdge = main.ViewportToWorldPoint(new Vector3(1f, 0f, distanceForZ)).x;
        
        // Get the half the width of the sprites so that they don't cross the edges partially before switching directions
        float halfWidth = GetComponent<SpriteRenderer>().bounds.extents.x + wallStepDetectionDistance;
        
        float xPos = transform.position.x;
        
        // If at the edge start an event
        if (xPos + halfWidth >= rightEdge)
        {
            if (!hitWall)
            {
                hitWall = true;
                OnEnemyTouchBorder?.Invoke(1); // 1 is for the right wall
            }
        } else if (xPos - halfWidth <= leftEdge)
        {
            if (!hitWall)
            {
                hitWall = true;
                OnEnemyTouchBorder?.Invoke(-1); // -1 is for the left wall
            }
        }
        else
        {
            hitWall = false;
        }
    }
    
    
    // Going to be called from LevelParser.cs when the enemy gets instantiated
    public void SetType(int type)
    {
        enemyType = type;
        // Set the value in the animator so that the enemy iterates througth the right sprites
        GetComponent<Animator>().SetInteger("Enemy Type", enemyType);    
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ouch!");
        
        // todo - destroy the bullet
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Bullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject); // this.gameObject does the same

            if (gameObject.CompareTag("30 Points"))
            {
                OnEnemyDied?.Invoke(30); // 30 Point Enemy
            } else if (gameObject.CompareTag("20 Points"))
            {
                OnEnemyDied?.Invoke(20); // 20 Point Enemy
            }
            else // 10 Point Enemy
            {
                OnEnemyDied?.Invoke(10); // Question Mark = if null, don't
            }
        }
        
        // todo - trigger death animation
    }

    public void PlayTicSound()
    {
        GetComponent<AudioSource>().PlayOneShot(enemyTic);
        
        // Debug.Log("Tic");
    }
    
    public void PlayTacSound()
    {
        GetComponent<AudioSource>().PlayOneShot(enemyTac);

        // Debug.Log("Tac");
    }
}
