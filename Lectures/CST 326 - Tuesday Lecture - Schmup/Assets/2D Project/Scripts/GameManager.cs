using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("GUI")]
    public GameObject loadingGUI; // UI that is displayed when game loads
    public GameObject inGameGUI; // UI that is displayed when playing the game

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI loadingHighScoreText;

    public static int scoreCount = 0;
    public static int highScoreCount = 0;
    
    public LevelParser levelParser;
    public BarrierSpawner barrierSpawner;
    public GameObject playerTank;
    
    public float scoreGUIDuration = 3f;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        if (loadingGUI != null)
        {
            loadingGUI.SetActive(true);
        }

        if (inGameGUI != null)
        {
            inGameGUI.SetActive(false);
        }

        if (playerTank != null)
        {
            playerTank.SetActive(false);
        }
        
        // Load high score by using PlayerPrefs which should save the high score between sessions (like the document says)
        highScoreCount = PlayerPrefs.GetInt("HI_SCORE", 0);

        scoreCount = 0;
        
        // Update the UI
        UpdateScoreUI();
        
        // Sign up for notification about enemy death 
        Enemy.OnEnemyDied += OnEnemyDied; // Suscribe
        UFO.OnUFOHit += OnUFOHit; // Suscribe
        
        StartCoroutine(startingUI()); // Start the coroutine for the score GUI
    }

    void OnDestroy()
    {
        Enemy.OnEnemyDied -= OnEnemyDied; // Unsuscribe
        UFO.OnUFOHit -= OnUFOHit; // Unsuscribe
    }
    
    void OnEnemyDied(float score)
    {
        AddPoints((int)score);
    }
    
    void OnUFOHit(int points)
    {
        AddPoints(points);
    }

    public static void AddPoints(int scoreToAdd)
    {
        scoreCount += scoreToAdd;

        if (scoreCount > highScoreCount)
        {
            highScoreCount = scoreCount;
            PlayerPrefs.SetInt("HI_SCORE", highScoreCount);
            PlayerPrefs.Save();
        }

        if (instance != null)
            instance.UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        // These two are for when the game is being played
        if (scoreText != null)
            scoreText.text = $"SCORE\n{scoreCount:0000}";

        if (highScoreText != null)
            highScoreText.text = $"HI-SCORE\n{highScoreCount:0000}";
        
        // This one is for the loading screen GUI (high score only)
        if (loadingHighScoreText != null)
            loadingHighScoreText.text = $"HI-SCORE\n{highScoreCount:0000}";
    }

    
    private static GameManager instance;
    
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
        
        
        // Hide the -- Loading -- GUI and show the -- In Game -- GUI
        if (loadingGUI != null) loadingGUI.SetActive(false);
        if (inGameGUI != null) inGameGUI.SetActive(true);
        
        // Proceed with placing all the enemies in their right positions
        levelParser.LoadLevel();
        
        // Proceed with placing the destructible barriers in the right spots
        barrierSpawner.SpawnBarriers();
        
        // Proceed with spawning the player
        playerTank.SetActive(true);

    }
}
