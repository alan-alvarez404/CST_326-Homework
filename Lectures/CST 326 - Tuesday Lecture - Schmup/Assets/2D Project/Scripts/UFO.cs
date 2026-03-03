using UnityEngine;

public class UFO : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip UFODown;
    
    // The score the UFO can give is chosen at random from this array
    public int[] numbers = { 100, 50, 50, 100, 150, 100, 100, 50, 300, 100, 100, 100, 50, 150, 100, 50 };
    
    public delegate void UFODiedFUnc(int randomPoints); // Func is delegate type
    public static event UFODiedFUnc OnUFOHit;

    private float speed = 5f;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D collider;
    private bool isActive = false;
    private Coroutine UFOCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        HideUFO();
    }
    
    void OnEnable()
    {
        EnemyShmovement.OnEnemiesSteppedDown += summonTheUFO;
    }

    void OnDisable()
    {
        EnemyShmovement.OnEnemiesSteppedDown -= summonTheUFO;
    }

    void summonTheUFO()
    {
        if (Random.Range(1, 4) != 1) return; // This sort of gives the UFO a 1 in 3 chance to actually appear like classic Space Invaders
        
        if (isActive)
        {
            return; 
        }
        
        if (UFOCoroutine != null) 
        {
            StopCoroutine(UFOCoroutine);
        }

        UFOCoroutine = StartCoroutine(MoveAcross());
    }
    
    System.Collections.IEnumerator MoveAcross()
    {
        isActive = true;
        ShowUFO();

        // Getting the camera for calculations
        Camera main = Camera.main;
        float distanceForZ = -main.transform.position.z; // Currently the camera's z in the inspector is -1, make it positive for future calculations
        
        // Left and Right Edges
        float leftEdge = main.ViewportToWorldPoint(new Vector3(0f, 0f, distanceForZ)).x;
        float rightEdge = main.ViewportToWorldPoint(new Vector3(1f, 0f, distanceForZ)).x;

        bool leftToRight = (Random.value < 0.5f);

        // The UFO should be able to spawn off screen thanks to this
        float halfWidth = spriteRenderer != null ? spriteRenderer.bounds.extents.x : 0.5f;
        float startingXPos = leftToRight ? (leftEdge - halfWidth - 0.2f) : (rightEdge + halfWidth + 0.2f);
        float endingXPos   = leftToRight ? (rightEdge + halfWidth + 0.2f) : (leftEdge - halfWidth - 0.2f);

        Vector3 position = transform.position;
        position.x = startingXPos;
        position.y = transform.position.y;
        transform.position = position;

        float dir = leftToRight ? 1f : -1f;

        while ((dir > 0f && transform.position.x < endingXPos) || (dir < 0f && transform.position.x > endingXPos))
        {
            transform.position += Vector3.right * (dir * speed * Time.deltaTime);
            yield return null;
        }

        HideUFO();
        isActive = false;
    }
    
    // Responsible for hiding and showing the UFO
    void HideUFO()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (collider != null) collider.enabled = false;
    }
    void ShowUFO()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (collider != null) collider.enabled = true;
    }

    int GetRandomPoints(int[] arrayOfPoints)
    {
        if (arrayOfPoints == null || arrayOfPoints.Length == 0)
        {
            return default; // This should never happen because the array always will be full
        }
        
        int randomIndex = Random.Range(0, arrayOfPoints.Length);
        return arrayOfPoints[randomIndex];
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // todo - destroy the bullet
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Bullet"))
        {
            Debug.Log("UFO was hit!");
            
            GetComponent<AudioSource>().PlayOneShot(UFODown);
            
            int mystery = GetRandomPoints(numbers);
            
            Destroy(collision.gameObject);

            if (gameObject.CompareTag("UFO"))
            {
                OnUFOHit?.Invoke(mystery); // Mystery points
                
                //GameManager.AddPoints(mystery); // Update the GUI
            }

            // Stop the enemy's collisions and shooting logic
            collider.enabled = false;
            
            // Don't destroy the UFO but hide it
            if (UFOCoroutine != null)
            {
                StopCoroutine(UFOCoroutine);
                UFOCoroutine = null;
            }
            HideUFO();
            isActive = false;
        }
    }
}
