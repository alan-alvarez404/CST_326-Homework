using UnityEngine;

public class WaterBlockLogic : MonoBehaviour
{
    // Prefab needed to make the player lose
    public GameObject waterBlockPrefab;
    
    public static void CheckForWater(CharacterController controller, Transform playerTransform)
    {
        // Center of the Mario
        Vector3 playerCenter = playerTransform.TransformPoint(controller.center);
        
        // Distance from that center to more than the bottom edge of the player
        float distance = (controller.height * 0.5f) + 0.5f;
        
        // Cast the ray to left, right, and down, then check for both flagpole prefabs
        if (Physics.Raycast(playerCenter, Vector3.down, out RaycastHit hit, distance))
        {
            // Checking for the water prefab
            if (hit.collider != null && hit.collider.CompareTag("Water"))
            {
                Lose();
            }
        }
    }
    
    // Callable method that handles stopping the timer and the Mario
    public static void Lose()
    {
        Debug.Log("Game Over: Fell into water");

        TimeController.StopTime();
        CharacterDriver.StopTheMario();
    }
}
