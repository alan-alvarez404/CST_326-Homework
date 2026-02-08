using UnityEngine;

public class GameState : MonoBehaviour
{
    // The whole purpose of this script is so that
    // when the game is loaded, a sound is played
    // and until the sound is over the game begins
    
    public static bool CanPlay { get; private set; }

    public static void gameCanPlay(bool canPlay)
    {
        CanPlay = canPlay;
    }
}