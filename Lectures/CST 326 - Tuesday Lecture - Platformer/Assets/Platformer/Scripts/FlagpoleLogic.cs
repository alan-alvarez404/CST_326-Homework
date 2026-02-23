using UnityEngine;

public class FlagpoleLogic : MonoBehaviour
{
    // The two objects that should trigger winning
    public GameObject flagTopPrefab;
    public GameObject flagPolePrefab;

    private static AudioController audioController;
    
    public static void CheckForWin(CharacterController controller, Transform playerTransform)
    {
        // Center of the Mario
        Vector3 playerCenter = playerTransform.TransformPoint(controller.center);
        
        // Distance from that center to more than the left and right edges of the player
        float distance = (controller.height * 0.5f) + 0.5f;
        
        // Cast the ray to left, right, and down, then check for both flagpole frefabs
        if (Physics.Raycast(playerCenter, Vector3.right, out RaycastHit hit, distance) || Physics.Raycast(playerCenter, Vector3.left, out hit, distance) || Physics.Raycast(playerCenter, Vector3.down, out hit, distance))
        {
            // Checking for two different prefabs that make up the flagpole
            if (hit.collider != null && (hit.collider.CompareTag("Flag Pole") || hit.collider.CompareTag("Flag Top")))
            {
                Win();
            }
        }
    }

    // Callable method that handles stopping the timer and the Mario
    public static void Win()
    {
        AudioController.Instance?.PlayMarioTouchesTheFlag(); // This will activate the coroutine
        // Where the sound of Mariou touching the flag plays, and after its over
        // the victory song will play.
        
        Debug.Log("You win!");
        TimeController.StopTime();
        CharacterDriver.StopTheMario(false, 0);
    }
}
