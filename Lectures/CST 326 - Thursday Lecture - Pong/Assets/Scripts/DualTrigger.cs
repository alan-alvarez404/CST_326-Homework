using UnityEngine;

public class DualTrigger : MonoBehaviour
{
    private LoadGame loadGame;
    private BallBehaviour ballBehaviour;
    private BallBehaviour ball;

    private void Awake()
    {
        if (loadGame == null)
        {
            loadGame = FindFirstObjectByType<LoadGame>();
        }

        if (ball == null)
        {
            ball = FindFirstObjectByType<BallBehaviour>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
        {
            return;
        }

        if (loadGame == null || ball == null)
        {
            Debug.LogError("Missing LoadGame or BallBehaviour reference.");
            return;
        }

        if (loadGame.ballSpawn == null)
        {
            Debug.LogError("LoadGame.ballSpawn is NOT assigned in the Inspector.");
            return;
        }

        if (CompareTag("LeftGoal"))
        {
            loadGame.AddPointToRight();
            ball.resetBall(loadGame.ballSpawn.position);
        }
        else if (CompareTag("RightGoal"))
        {
            loadGame.AddPointToLeft();
            ball.resetBall(loadGame.ballSpawn.position);
        }
        else
        {
            Debug.LogWarning("DualTrigger is not LeftGoal/RightGoal: " + gameObject.name);
        }
    }





}
