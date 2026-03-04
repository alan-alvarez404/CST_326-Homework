using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public void LoadGame()
    {
        StartCoroutine(_LoadGame());

        IEnumerator _LoadGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("2D Project/Scenes/Schmup");
            while (!loadOperation!.isDone) yield return null; // Exclamation mark after loadOperation makes the null reference exception shut up lmao

            // Wait until scene is locked and loaded, then find player
        
            GameObject playerObject = GameObject.Find("Player Tank");
            Debug.Log(playerObject.name);
        
            Debug.Log("Sees");
        }
    }
}
