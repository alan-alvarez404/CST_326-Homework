using UnityEngine;

public class DualTrigger : MonoBehaviour
{
    private LoadGame loadGame;
    
    // If we try to use these variables when there is a bonus ball
    // in the game field, if the bonus ball scores it'll get rid
    // of the main original ball. Have to comment these out
    // private BallBehaviour ballBehaviour;
    // private BallBehaviour ball;
    
    private void Awake()
    {
        if (loadGame == null)
        {
            loadGame = FindFirstObjectByType<LoadGame>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
        {
            return;
        }

        if (loadGame == null)
        {
            Debug.LogError("Missing LoadGame reference.");
            return;
        }

        BallBehaviour hitBall = other.GetComponent<BallBehaviour>();
        if (hitBall == null)
        {
            Debug.LogError("Missing BallBehaviour reference.");
            return;
        }

        int points = 1;
        if (hitBall.isBonus)
        {
            points = 3;
        }

        if (CompareTag("LeftGoal"))
        {
            loadGame.addPointsToRight(points);
        }
        else if (CompareTag("RightGoal"))
        {
            loadGame.addPointsToLeft(points);
        }
        else
        {
            Debug.LogWarning("DualTrigger is not LeftGoal/RightGoal: " + gameObject.name);
            return;
        }

        // What happens to the ball after scoring
        if (hitBall.isBonus)
        {
            Destroy(hitBall.gameObject);
        }
        else
        {
            if (loadGame.ballSpawn == null)
            {
                Debug.LogError("LoadGame.ballSpawn is NOT assigned in the Inspector.");
                return;
            }

            hitBall.resetBall(loadGame.ballSpawn.position);
        }
    }
}
