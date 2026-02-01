using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public float lastInputTime = 0f;
    public float inputDelay = 2f;
    private List<GameObject> spawnedBalls = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(ballPrefab);
        
        /*
        while (true)
        {
            Instantiate(ballPrefab);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        //if (Keyboard.current.spaceKey.isPressed && Time.deltaTime > lastInputTime + inputDelay)
        if(Keyboard.current.spaceKey.isPressed)
        {
            Vector3 basePos = transform.position;

            float randomX = Random.Range(-0.2f, 2.25f);
            Vector3 spawnPos = new Vector3(randomX, basePos.y, basePos.z);

            GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
            spawnedBalls.Add(ball);
        }
    }

    public void DestroyAllBalls()
    {
        foreach (GameObject ball in spawnedBalls)
        {
            if (ball != null)
            {
                Destroy(ball);
            }
            spawnedBalls.Clear();
        }
    }
}
