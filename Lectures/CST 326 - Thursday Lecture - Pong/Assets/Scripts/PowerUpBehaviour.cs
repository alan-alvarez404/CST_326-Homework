using System.Collections;
using UnityEngine;

public class PowerUpBehaviour : MonoBehaviour
{
    // The three different powerup variations
    public enum PowerUpType
    {
        SlowButBig,
        BiggerPaddles,
        BonusBall
    }
    
    private AudioController audioController;
    
    private PowerUpType type =  PowerUpType.SlowButBig;
    private bool randomizeType = true;
    
    // Spin the powerup around
    private Vector3 rotationSpeed = new Vector3(0f, 0f, 90f);

    // For the SlowButBig powerup
    public float ballScaleMultiplier = 1.02f;
    private float ballSpeedMultiplier = 0.5f;
    
    // For the BiggerPaddles powerup
    private float paddleScaleMultiplier = 1.25f;
    private float paddleDuration = 10f;

    private bool used;

    private void Awake()
    {
        audioController = FindFirstObjectByType<AudioController>();
    }
    
    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used)
        {
            return;
        }

        if (!other.CompareTag("Ball"))
        {
            return; // Must only collide with Ball for powerups to take place
        }
        
        BallBehaviour ball = other.GetComponent<BallBehaviour>();
        if (ball == null)
        {
            Debug.LogWarning("PowerUpBehaviour: Current object " + other.name + " has no BallBehaviour");
            return;
        }

        used = true; // Powerup is marked as used
        
        if (audioController != null)
        {
            audioController.PlayPowerUpCollect(); // Play the sound for all three powerups
        }

        PowerUpType chosen = type;
        if (randomizeType) // Should always be true
        {
            int choice = Random.Range(0, 3);
            if (choice == 0)
            {
                chosen = PowerUpType.SlowButBig;
            }
            else if (choice == 1)
            {
                chosen = PowerUpType.BiggerPaddles;
            }
            else
            {
                chosen = PowerUpType.BonusBall;
            }
        }

        if (chosen == PowerUpType.SlowButBig)
        {
            ball.changeSizeAndSpeed(ballScaleMultiplier, ballSpeedMultiplier);
            Destroy(gameObject); // Destroy the powerup
        } else if (chosen == PowerUpType.BiggerPaddles)
        {
            // This isn't destroying the powerup because if we did it would get rid of the
            // StartCoroutine functionality along with it, making this powerup non-functional
            // It basically disables all collisions and rendering of the powerup object
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Renderer rend = GetComponent<Renderer>();
            if (rend != null) rend.enabled = false;
            
            StartCoroutine(ApplyBigPaddles()); // I looked online for some kind of function that
            // Would essentially do something for only a set while and then undo its action
            // Basically the ball will increase in size, wait 10 secs, then decrease
            // Turns out this StartCoroutine is a good way of doing so0
        } else if (chosen == PowerUpType.BonusBall)
        {
            ball.spawnBonusBall();
            Destroy(gameObject); // Destroy the powerup
        }
    }

    // This IEnumarator has to be used for the StartCoroutine to work
    private IEnumerator ApplyBigPaddles()
    {
        GameObject[] paddles = GameObject.FindGameObjectsWithTag("Paddle");
        if (paddles == null || paddles.Length == 0)
        {
            Destroy(gameObject); // Get rid of the powerup
            yield break;
        }

        Vector3[] original = new Vector3[paddles.Length];

        int i = 0;
        while (i < paddles.Length)
        {
            original[i] = paddles[i].transform.localScale;
            paddles[i].transform.localScale = original[i] * paddleScaleMultiplier;
            i++;
        }

        // This is essentially the part of the function that pauses
        // itself and waits 10 seconds (paddleDuration) to complete
        // the second half (return the paddles to normal size)
        yield return new WaitForSeconds(paddleDuration);

        i = 0;
        while (i < paddles.Length)
        {
            if (paddles[i] != null)
            {
                paddles[i].transform.localScale = original[i];
            }
            i++;
        }
        Destroy(gameObject); // Get rid of the powerup
    }
}
