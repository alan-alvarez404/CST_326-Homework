using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public LoadGame loadGame;

    public TMP_Text leftScoreText;
    public TMP_Text leftStatusText;
    
    public TMP_Text rightScoreText;
    public TMP_Text rightStatusText;

    private int lastLeft = -999;
    private int lastRight = -999;
    private bool lastGameOver;

    private void Awake()
    {
        if (loadGame == null)
        {
            loadGame = FindFirstObjectByType<LoadGame>();
        }
    }

    private void Update()
    {
        if (loadGame == null)
        {
            return;
        }

        // Update scores only when they change
        if (loadGame.leftScore != lastLeft)
        {
            lastLeft = loadGame.leftScore;
            leftScoreText.text = "Player 1 Score: " + lastLeft;
        }

        if (loadGame.rightScore != lastRight)
        {
            lastRight = loadGame.rightScore;
            rightScoreText.text = "Player 2 Score: " + lastRight;
        }

        // Update the win/lose text when game over
        if (GameState.GameOver != lastGameOver)
        {
            lastGameOver = GameState.GameOver;

            if (!lastGameOver)
            {
                SetLeftStatus("");
                SetRightStatus("");
            }
            else
            {
                bool leftWon = loadGame.leftScore >= 11;
                bool rightWon = loadGame.rightScore >= 11;

                if (leftWon && !rightWon)
                {
                    SetLeftStatus("You Win!");
                    SetRightStatus("You Lose!");
                }
                else if (rightWon && !leftWon)
                {
                    SetLeftStatus("You Lose!");
                    SetRightStatus("You Win!");
                }
                else
                {
                    // For whatever reason and this has only happened once so far,
                    // the status text You Win or You Lose never showed up so I
                    // added a just in case if that ever happens so the player
                    // knows that the match is over
                    // It might be a hidden bug somewhere, but I don't know what
                    SetLeftStatus("Game Over");
                    SetRightStatus("Game Over");
                }
            }
        }
    }

    private void SetLeftStatus(string s)
    {
        if (leftStatusText != null)
        {
            leftStatusText.text = s;
        }
    }

    private void SetRightStatus(string s)
    {
        if (rightStatusText != null)
        {
            rightStatusText.text = s;
        }
    }
}
