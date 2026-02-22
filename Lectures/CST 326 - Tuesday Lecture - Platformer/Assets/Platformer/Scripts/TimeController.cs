using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    public float timeLeft = 100f; // 400 is for classic mario levels
    // Have to do 100 seconds per the assignment rules
    
    private bool gameOver = false;
    
    // Have to do this to be able to stop time in the next method properly
    private static TimeController instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    // Callable method to stop time
    public static void StopTime()
    {
        instance.enabled = false; // Properly stop update from counting down the timer
    }
    
    void Update()
    {
        if (timeLeft <= 0)
        {
            timeLeft = 0f;
            timeText.text = $"TIME\n {((int)timeLeft).ToString()}";

            if (!gameOver)
            {
                Debug.Log("Game Over: Ran out of time");
                gameOver = true;
            }
            return;
        }
        
        timeLeft -= Time.deltaTime * 1f; // * 3 for accurate time countdown from the original game
        timeText.text = $"TIME\n {((int)timeLeft).ToString()}";
    }
}
