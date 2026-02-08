using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LoadGame : MonoBehaviour
{
    public int leftScore = 0;
    public int rightScore = 0;

    public Transform ballSpawn;

    public void addPointsToLeft(int points)
    {
        leftScore += points;
        Debug.Log("Player 1 scored! Current score: " + leftScore);

        if (leftScore >= 11)
        {
            Debug.Log("Game Over, Left Paddle Wins");
            leftScore = 0;
            rightScore = 0;
        }
    }

    public void addPointsToRight(int points)
    {
        rightScore += points;
        Debug.Log("Player 2 scored! Current score: " + rightScore);
        if (rightScore >= 11)
        {
            Debug.Log("Game Over, Right Paddle Wins");
            leftScore = 0;
            rightScore = 0;
        }
    }
}
