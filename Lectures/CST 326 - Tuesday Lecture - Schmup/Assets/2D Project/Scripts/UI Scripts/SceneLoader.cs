using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;

    void Awake()
    {
        // As a failsafe so there aren't errors relating to trying to access the main menu scene while in another scene
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(gameObject);
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Player.OnPlayerDied += OnPlayerDeath; // Suscribe to player death event
        EnemyShmovement.OnEnemiesDestroyed += OnEnemiesCleared; // Suscribe to enemies cleared event
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Player.OnPlayerDied -= OnPlayerDeath; // Unsuscribe from player death event
        EnemyShmovement.OnEnemiesDestroyed -= OnEnemiesCleared; // Unsuscribe from enemies cleared event
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetEnemyTypes();
        RewireStartButton(); // Have to do this so that the start button works again
    }

    private void RewireStartButton()
    {
        // Find the button by name
        var startButton = GameObject.Find("Start Button");
        if (startButton == null) return;
        
        var button = startButton.GetComponent<Button>();
        if (button == null) return;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(LoadGame);
    }
    
    void Start()
    {
        // Called once the main menu loads
        SetEnemyTypes();
    }

    private void SetEnemyTypes()
    {
        // Make sure this is only done in the main menu
        if (SceneManager.GetActiveScene().name != "Main Menu")
            return;
        
        // Getting the enemies by looking for the enemy objects inside the canvas that have the EnemyUI script attatched
        var enemies = FindObjectsByType<EnemyUI>(FindObjectsInactive.Include, FindObjectsSortMode.None); // Doing some black magic stuff here
        
        // This for loop handles calling the SetType function and passing in the right variable for the animation
        foreach (var i in enemies)
        {
            if (i.gameObject.name.StartsWith("10")) i.SetEnemyType(1);
            else if (i.gameObject.name.StartsWith("20")) i.SetEnemyType(2);
            else if (i.gameObject.name.StartsWith("30")) i.SetEnemyType(3);
        }
    }
    
    public void LoadGame()
    {
        StartCoroutine(_LoadGame());

        IEnumerator _LoadGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("2D Project/Scenes/Schmup");
            while (!loadOperation!.isDone) yield return null; // Exclamation mark after loadOperation makes the null reference exception shut up lmao

            // Wait until scene is locked and loaded
        
            Debug.Log("Loaded Game");
        }
    }

    // Will handle loading the game to the credits scene upon the player getting their tank destroyed
    public void OnPlayerDeath()
    {
        StartCoroutine(_LoadCreditsCoroutine(false));
    }
    
    // Will handle loading the credits after receiving the event that all of the enemies have been cleared
    public void OnEnemiesCleared()
    {
        StartCoroutine(_LoadCreditsCoroutine(true));
    }

    public void LoadMainMenu(int delay)
    {
        StartCoroutine(LoadMainMenuCoroutine(delay));
    }

    private IEnumerator _LoadCreditsCoroutine(bool won)
    {
        yield return new WaitForSeconds(1);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("2D Project/Scenes/Credits");
        while (!loadOperation!.isDone) yield return null; // Exclamation mark after loadOperation makes the null reference exception shut up lmao

        // Wait until scene is locked and loaded
        
        Debug.Log("Loaded Credits");
        
        // If the player won or not
        if (won)
        {
            Debug.Log("You Won!");
        }
        else
        {
            Debug.Log("You Lost");
        }

        // Run after the credits are shown for 5 seconds
        LoadMainMenu(5);
    }
    
    private IEnumerator LoadMainMenuCoroutine(int delay)
    {
        yield return new WaitForSeconds(delay); // Wwait the 5 seconds that the credits lasts
        SceneManager.LoadSceneAsync("2D Project/Scenes/Main Menu");
        
        Debug.Log("Loaded Main Menu");
    }
}
