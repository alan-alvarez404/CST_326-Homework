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
            Transform myTransform = GetComponent<Transform>();
            GameObject ball = Instantiate(ballPrefab, myTransform.position, Quaternion.identity);
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
