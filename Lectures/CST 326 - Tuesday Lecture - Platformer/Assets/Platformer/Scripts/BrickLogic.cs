using UnityEngine;
using UnityEngine.InputSystem;

public class BrickLogic : MonoBehaviour
{
    public Camera rayCamera;

    void Awake()
    {
        if (rayCamera == null)
        {
            rayCamera = Camera.main; // Assign the main camera if not assigned yet
        }
    }
    
    void Update()
    {
        Vector3 mousePosition = Mouse.current.position.value;
        Ray screenRay = rayCamera.ScreenPointToRay(mousePosition);
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics.Raycast(screenRay, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("Brick"))
                {
                    // Destroy any object tagged Brick if ray intersects with it
                    Destroy(hit.collider.transform.gameObject);
                    
                    // Add score
                    ScoreCounter.AddScore(100);

                    Debug.Log("Brick at Position: " + hit.transform.position + " was destroyed"); // Print the position of whatever brick was destroyed
                }
            }
        }
    }
        
    // Will be called by another script
    public void Break()
    {
        ScoreCounter.AddScore(100);
        Destroy(gameObject);
    }
    
    public static void CheckForBrick(CharacterController controller, Transform playerTransform)
    {
        // Center of the Mario
        Vector3 playerCenter = playerTransform.TransformPoint(controller.center);
        
        // Distance from that center to a little above the Mario (needed for the following)
        float distance = (controller.height * 0.5f) + 0.1f;
        
        // Cast the ray directly upwards, check for brick
        if (Physics.Raycast(playerCenter, Vector3.up, out RaycastHit hit, distance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Brick"))
            {
                ScoreCounter.AddScore(100);
                Destroy(hit.collider.transform.gameObject);
            }
        }
        
        Debug.DrawRay(playerCenter, Vector3.up * distance, Color.red, 0.1f);
    }
}
