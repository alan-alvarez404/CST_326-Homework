using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LoadGame : MonoBehaviour
{
    public int leftScore = 0;
    public int rightScore = 0;

    public Transform ballSpawn;

    public float fadeDuration = 5f;

    public void addPointsToLeft(int points)
    {
        // This is so that points cannot get added once the game is over
        // The one moment where this can occur is if both the bonus ball
        // and the normal ball score right at the same exact moment.
        // This would trigger the player win sound twice.
        if (GameState.GameOver)
        {
            return;
        }
        
        leftScore += points;
        Debug.Log("Player 1 scored! Current score: " + leftScore);

        if (leftScore >= 11)
        {
            // Going to move the entirity of what happens inside
            // this if statement into its own function just to
            // be reusable.
            // Pass in a debug statement
            gameIsWonCleanUp("Game Over, Left Paddle Wins");
        }
    }

    public void addPointsToRight(int points)
    {
        // This is so that points cannot get added once the game is over
        // The one moment where this can occur is if both the bonus ball
        // and the normal ball score right at the same exact moment.
        // This would trigger the player win sound twice.
        if (GameState.GameOver)
        {
            return;
        }
        
        rightScore += points;
        Debug.Log("Player 2 scored! Current score: " + rightScore);
        
        if (rightScore >= 11)
        {
            // Going to move the entirity of what happens inside
            // this if statement into its own function just to
            // be reusable.
            // Pass in a debug statement
            gameIsWonCleanUp("Game Over, Right Paddle Wins");
        }
    }
    
    
    private void gameIsWonCleanUp(string message)
    {
        Debug.Log(message);

        // Game is over
        GameState.gameOver(true);

        // Play the winning sound and fade away the music
        if (AudioController.Instance != null)
        {
            AudioController.Instance.PlayPlayerWin();
            AudioController.Instance.FadeOutBackgroundMusic(fadeDuration);
        }

        // Hopefully slow down the skybox to a complete stop
        RotateSkybox sky = FindFirstObjectByType<RotateSkybox>();
        if (sky != null)
        {
            sky.slowDownSkybox(fadeDuration);
        }

        // Reset and freeze the paddles
        PongPaddle[] paddles = FindObjectsByType<PongPaddle>(FindObjectsSortMode.None);
        int i = 0;
        while (i < paddles.Length)
        {
            if (paddles[i] != null)
            {
                paddles[i].resetAndFreezePosition();
            }
            i++;
        }

        // Reset and freeze the ball
        BallBehaviour[] balls = FindObjectsByType<BallBehaviour>(FindObjectsSortMode.None);
        i = 0;
        while (i < balls.Length)
        {
            if (balls[i] != null)
            {
                balls[i].resetAndFreezePosition();
            }
            i++;
        }

        // leftScore = 0;
        // rightScore = 0;
    }
}
