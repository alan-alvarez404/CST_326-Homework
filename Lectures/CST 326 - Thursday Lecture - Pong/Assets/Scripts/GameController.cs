using UnityEngine;

public class GameState : MonoBehaviour
{
    // The whole purpose of this script is so that
    // when the game is loaded, a sound is played
    // and until the sound is over the game begins
    
    public static bool CanPlay { get; private set; }
    
    // For when the game is over
    public static bool GameOver { get; private set; }

    public static void gameCanPlay(bool canPlay)
    {
        CanPlay = canPlay;
    }

    public static void gameOver(bool gameOver)
    {
        GameOver = gameOver;

        if (gameOver)
        {
            CanPlay = false; // Ensure the game can no longer be playable
        }
    }

    public static void ResetGameState()
    {
        CanPlay = false;
        GameOver = false;
    }
}