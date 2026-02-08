using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;

    public float spawnTime = 10f; // In seconds

    private float minz = -20f;
    private float maxz = 20f;
    private float minY = -12f;
    private float maxY = 12f;
    
    public bool spawnOnStart = false;

    private float spawnTimer;

    private void Start()
    {
        spawnTimer = 0f;
        if (spawnOnStart)
        {
            SpawnPowerUp();
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnTime)
        {
            spawnTimer = 0f;
            SpawnPowerUp();
        }
    }

    private void SpawnPowerUp()
    {
        if (powerUpPrefab == null)
        {
            Debug.LogError("PowerUpSpawner: powerUpPrefab has not been assigned.");
            return;
        }
        
        float y = Random.Range(minY, maxY);
        float z = Random.Range(minz, maxz);
        
        Vector3 pos = new Vector3(0f, y, z);
        Instantiate(powerUpPrefab, pos, Quaternion.identity);
    }
}
