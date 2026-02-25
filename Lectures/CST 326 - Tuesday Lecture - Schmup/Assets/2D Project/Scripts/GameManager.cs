using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
       // todo - sign up for notification about enemy death 
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
}
