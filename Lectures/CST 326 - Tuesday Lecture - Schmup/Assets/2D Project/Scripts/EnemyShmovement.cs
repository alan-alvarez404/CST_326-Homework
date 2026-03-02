using System.Collections;
using UnityEngine;

public class EnemyShmovement : MonoBehaviour
{
    [Header("Movement Intervals")]
    public float stepIntervalInSeconds = 1f; // 1 is good (will be changed as less enemies there are)
    public float xStepDistance = 0.25f; // Reused from Enemy.cs, has to be 0.25
    public float yStepDistance = 0.25f;

    private int currentStepDirection = 1; // 1 to the right, -1 to the left
    private bool okayToStepDown = false;

    // Fire an event that Enemy.cs will listen for so that it can attempt to fire a bullet
    public delegate void EnemySteppedFunc(); // Func is delegate type
    public static event EnemySteppedFunc OnEnemyStep;
    
    void OnEnable()
    {
        Enemy.OnEnemyTouchBorder += enemyBorderToucher;
    }

    void OnDisable()
    {
        Enemy.OnEnemyTouchBorder -= enemyBorderToucher;
    }

    void Start()
    {
        // Makes it so the step distance here affects the step detection in Enemy.cs
        Enemy.wallStepDetectionDistance = xStepDistance; // Just in case : )
        StartCoroutine(Stepping()); // Using a coroutine for handling enemy steps
    }
    
    private void enemyBorderToucher(int whichWall)
    {
        okayToStepDown = true;
    }
    
    // This should return an updated step interval based on how much enemies are left
    private float newStepInterval()
    {
        int alive = transform.childCount;
        
        if (alive <= 5)  return stepIntervalInSeconds * 0.12f;
        if (alive <= 10) return stepIntervalInSeconds * 0.20f;
        if (alive <= 20) return stepIntervalInSeconds * 0.36f;
        if (alive <= 35) return stepIntervalInSeconds * 0.60f;
        
        return stepIntervalInSeconds * 1.00f;
    }

    // Doing local position since that's how we got it to work for instantiating them
    private IEnumerator Stepping()
    {
        while (true) // Always will happen
        {
            yield return new WaitForSeconds(newStepInterval()); // Should be a 1.0 second interval

            if (okayToStepDown)
            {
                okayToStepDown = false;
                currentStepDirection *= -1;
                
                // Move the entire group of enemies down a step
                foreach (Transform child in transform)
                {
                    child.localPosition += Vector3.down * yStepDistance;
                }
                
                Debug.Log("Enemies touched wall. Now stepping down");
            }
            
            // Move the group of enemies left or right
            foreach (Transform child in transform)
            {
                child.localPosition += Vector3.right * (xStepDistance * currentStepDirection);
            }

            // Fire the event that Enemy.cs will listen for
            OnEnemyStep?.Invoke();
        }
    }
}
