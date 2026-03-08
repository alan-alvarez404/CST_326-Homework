using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    public Transform shootOffsetTransform;
    
    [Header("Audio")]
    public AudioClip enemyFire;
    public AudioClip enemyDie;

    public int enemyType;

    [Header("Animation Parameters")] 
    public float deathDelay = 0.25f;
    private bool isDying = false;
    private Animator animator;
    private new Collider2D collider;

    public static float wallStepDetectionDistance = 0.25f; // Shared within the enemy movement script
    
    public delegate void EnemyDiedFunc(float points); // Func is delegate type
    public static event EnemyDiedFunc OnEnemyDied;
    
    // Making a new delegate and event for when an enemy reaches the rightmost or leftmost sides of the game
    public delegate void EnemyTouchedWallFunc(int whichWall);
    public static event EnemyTouchedWallFunc OnEnemyTouchBorder;
    
    private bool hitWall = false;
    
    [Header ("Shooting Parameters")]
    public float shootDuration = 0.75f; // 0.75 is a good duration for the length that the shooting animation takes (even though it's only 4 frames)
    public float fireBulletDelay = 0.75f; // How much time the enemy should wait before firing (in the middle of the shooting animation) (0.75 should also be good here)
    private Coroutine shootingCoroutine;
    
    // Static variables are associated with the object
    // It's associated with a class definition and not an instance (these are notes btw)

    void Update()
    {
        CheckForWall(); // Check for left or right wall every frame
    }

    void Awake()
    {
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        animator = (spriteRenderer != null) ? spriteRenderer.GetComponent<Animator>() : null;
        collider = GetComponent<Collider2D>();
        
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
        if (isDying) return;
        
        // Prevent trying to shoot again while in the middle of a shooting animation
        if (shootingCoroutine != null) return;
        
        int randValue = Random.Range(1, 11); // Generate random num b

        if (randValue == 1)
        {
            if (animator != null)
            {
                animator.SetBool("Is Shooting", true);
                // The shooting animation for each enemy should occur cuz of this trigger
                animator.SetTrigger("Enemy Shot Trigger");

                // Call the coroutine that will handle shooting at a certain point in the shooting animation
                shootingCoroutine = StartCoroutine(ShootSequence());
            }
        }
    }
    
    private IEnumerator ShootSequence()
    {
        // Wait the delay for shooting in the middle of the animation
        yield return new WaitForSeconds(fireBulletDelay);

        // Reweriting this so that it fires the bullet and returns a bool when its fired
        bool fired = Bullet.ShootBullet(enemyBulletPrefab, shootOffsetTransform.position, true);
        if (fired)
        {
            GetComponent<AudioSource>().PlayOneShot(enemyFire);
            Debug.Log("Enemy Fired!");
        }

        // Wait the rest of the enemy shooting animation by subtracting from the fireBulletDelay
        float remaining = Mathf.Max(0f, shootDuration - fireBulletDelay);
        yield return new WaitForSeconds(remaining);

        // Setting this back to false should allow the enemy to back to its idle animation
        animator.SetBool("Is Shooting", false);
        shootingCoroutine = null;
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
        if (animator != null)
        {
            animator.SetInteger("Enemy Type", enemyType);    
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ouch!");
        
        // todo - destroy the bullet
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Bullet") && !isDying)
        {
            isDying = true;
            
            Destroy(collision.gameObject);

            if (gameObject.CompareTag("30 Points"))
            {
                GetComponent<AudioSource>().PlayOneShot(enemyDie);
                
                OnEnemyDied?.Invoke(30); // 30 Point Enemy
                
                // GameManager.AddPoints(30); // Update the GUI

            } else if (gameObject.CompareTag("20 Points"))
            {
                GetComponent<AudioSource>().PlayOneShot(enemyDie);
                
                OnEnemyDied?.Invoke(20); // 20 Point Enemy
                
                // GameManager.AddPoints(20); // Update the GUI

            }
            else // 10 Point Enemy
            {
                GetComponent<AudioSource>().PlayOneShot(enemyDie);

                OnEnemyDied?.Invoke(10); // Question Mark = if null, don't
                
                // GameManager.AddPoints(10); // Update the GUI

            }
            
            // Stop the enemy's collisions and shooting logic
            collider.enabled = false;
            EnemyShmovement.OnEnemyStep -= TryToShoot;
            
            // Make it so that it can no longer be moved by EnemyShmovement.cs
            transform.SetParent(null, true);
            
            // Trigger death animation
            if (animator != null)
            {
                animator.SetBool("Is Dead", true); // Setting this to true should ensure that the idle states
                // no longer try to happen which may or may not have been overwriting the death animation
                // Yep that solves it
                animator.SetTrigger("Enemy Died");
            }
            
            Destroy(gameObject, deathDelay); // Destroy the enemy after a set delay
        }
    }
}
