using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public static int scoreCount;
    public TextMeshProUGUI scoreCountText;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure the score counter is padded with the right amount of 0's
        scoreCount = 0; 
        scoreCountText.text = $"Mario\n{scoreCount:000000}";
    }

    // Method to be called to add score
    public static void AddScore(int amount)
    {
        scoreCount += amount;
        Debug.Log($"Score: {scoreCount}"); // Debug for now
    }
}
