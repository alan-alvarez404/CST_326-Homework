using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        // Only react to the local player (tagged "Player")
        if (other.CompareTag("Player"))
            gameManager.OnLocalPlayerFinished();
    }
}