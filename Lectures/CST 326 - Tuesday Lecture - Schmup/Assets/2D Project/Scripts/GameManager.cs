using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject scoreGUI;
    public LevelParser levelParser;
    public float scoreGUIDuration = 3f;
    
    void Start()
    {
        if (scoreGUI != null)
        {
            scoreGUI.SetActive(true);
        }

        StartCoroutine(startingUI()); // Start the coroutine for the score GUI
        
       // Sign up for notification about enemy death 
       Enemy.OnEnemyDied += OnEnemyDied; // Suscribe
    }

    void OnDestroy()
    {
        Enemy.OnEnemyDied -= OnEnemyDied; // Unsuscribe
    }
    
    void OnEnemyDied(float score)
    {
        Debug.Log($"Killed enemy worth {score}");
    }

    // This handles the score GUI being displayed for 3 seconds as long as there's no lmb click
    IEnumerator startingUI()
    {
        float timer = 0f; // Use this to count up
        while (timer < scoreGUIDuration)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                break; // Stop checking and go to disabling the GUI
            }
            
            timer += Time.deltaTime;
            yield return null;
        }

        // Disable the score GUI
        scoreGUI.SetActive(false);
        
        // Proceed with placing all the enemies in their right positions
        levelParser.LoadLevel();
    }
}
