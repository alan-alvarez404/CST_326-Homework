using UnityEngine;

public class LoadGame : MonoBehaviour
{
    public int leftScore = 0;
    public int rightScore = 0;

    public Transform ballSpawn;

    public void AddPointToLeft()
    {
        leftScore++;
        Debug.Log("Player 1 scored! Current score: " + leftScore);
    }

    public void AddPointToRight()
    {
        rightScore++;
        Debug.Log("Player 2 scored! Current score: " + rightScore);
    }
}
