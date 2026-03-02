using UnityEngine;

public class BarrierSpawner : MonoBehaviour
{
    public GameObject barrierPrefab;

    public float yDistanceFromBottom = 2.5f; // 2.5 is good
    public float xHorizontalPosition = 1.0f;

    void Start()
    {
        // Will be called in GameManager.cs
        // SpawnBarriers();
    }

    public void SpawnBarriers()
    {
        if (barrierPrefab == null)
        {
            return;
        }
        
        // Getting the camera for calculations
        Camera main = Camera.main;
        float distanceForZ = -main.transform.position.z; // Currently the camera's z in the inspector is -1, make it positive for future calculations
        
        // Left and Right Edges
        float leftEdge = main.ViewportToWorldPoint(new Vector3(0f, 0f, distanceForZ)).x;
        float rightEdge = main.ViewportToWorldPoint(new Vector3(1f, 0f, distanceForZ)).x;
        float bottomEdge = main.ViewportToWorldPoint(new Vector3(0f, 0f, distanceForZ)).y;
        
        // Only the width that the barriers should be able to spawn in
        float barrierSpawnWidth = (rightEdge - leftEdge) - (xHorizontalPosition * 2f);
        float barrierSpacing = barrierSpawnWidth / 3f; // Gaps between 4 barriers
        
        float distanceForY = bottomEdge + yDistanceFromBottom;
        
        for (int i = 0; i < 4; i++)
        {
            float x = leftEdge + xHorizontalPosition + (barrierSpacing * i);

            GameObject b = Instantiate(barrierPrefab, new Vector3(x, distanceForY, -1), Quaternion.identity);
            b.name = $"Barrier {i + 1}";

            Barrier barrier = b.GetComponent<Barrier>();
            if (barrier != null)
                barrier.whichBarrier = i + 1;
        }
    }
}
