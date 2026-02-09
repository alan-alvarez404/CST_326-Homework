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
            // Play the audio
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayPlayerWin();
            }
            
            Debug.Log("Game Over, Left Paddle Wins");
            // leftScore = 0;
            // rightScore = 0;
            
            GameState.gameOver(true); // Game is over
            AudioController.Instance.FadeOutBackgroundMusic(fadeDuration); // Fade out the music
            
            // This is so that the skybox slows down at the same rate the music fades away
            RotateSkybox sky = FindFirstObjectByType<RotateSkybox>();
            if (sky != null) // Make sure a sky is assigned in the first place
            {
                sky.slowDownSkybox(fadeDuration);
            }
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
            // Play the audio
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayPlayerWin();
            }
            
            Debug.Log("Game Over, Right Paddle Wins");
            // leftScore = 0;
            // rightScore = 0;
            
            GameState.gameOver(true); // Game is over
            AudioController.Instance.FadeOutBackgroundMusic(fadeDuration); // Fade out the music
            
            // This is so that the skybox slows down at the same rate the music fades away
            RotateSkybox sky = FindFirstObjectByType<RotateSkybox>();
            if (sky != null) // Make sure a sky is assigned in the first place
            {
                sky.slowDownSkybox(fadeDuration);
            }
        }
    }
}
