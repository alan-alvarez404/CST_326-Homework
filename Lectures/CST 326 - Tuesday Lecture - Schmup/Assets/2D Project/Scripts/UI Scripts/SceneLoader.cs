using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetEnemyTypes();
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
}
