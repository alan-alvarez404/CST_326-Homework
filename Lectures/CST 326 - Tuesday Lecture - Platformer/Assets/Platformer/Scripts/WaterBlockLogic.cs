using UnityEngine;

public class WaterBlockLogic : MonoBehaviour
{
    // Needed for losing
    public float fallRate = -1f; // Rate at which he falls through (hopefully slow)
    public static float newFallRate; // Default should be -1 by the way. Looks the best
    
    // Prefab needed to make the player lose
    public GameObject waterBlockPrefab;

    // Have to do this just so I can edit the fall rate in the inspector
    private void Awake()
    {
        newFallRate = fallRate;
    }
    
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

        // This is needed so that Mario continues to fall through water at a slow rate
        bool fallThrough = true;
        
        // Debug.Log($"newFallRate = {newFallRate}");
        
        TimeController.StopTime();
        CharacterDriver.StopTheMario(fallThrough, newFallRate); // Pass in a value related to the y velocity
    }
}
