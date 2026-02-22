using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CoinCountLogic : MonoBehaviour
{
    public Camera rayCamera;
    
    // Doing this similarly to how I did it for the score counter
    public static int coinCount;
    public static TextMeshProUGUI coinText;
    public TextMeshProUGUI coinCountText;
    
    void Start()
    {
        // Set the initial coin counter to the proper Ox00 count
        coinCount = 0;
        coinText = coinCountText.GetComponent<TextMeshProUGUI>();
        coinText.text = $" x{coinCount:00}";
    }
    
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
                if (hit.collider != null && hit.collider.CompareTag("Question"))
                {
                    coinCount++; // Add 1 coin to the coin count
                    coinCountText.text = $"Ox{coinCount:00}"; // Update the coin display
                    // This stays as Ox00 up until it hits Ox10 and so on
                    
                    Debug.Log("Question Block at Position: " + hit.transform.position + " was hit"); // Print the position of whatever question block was hit
                    // Debug.DrawRay(screenRay.origin, screenRay.direction * 100f, Color.green, 0.5f); // Draw a debug ray for half a second
                }
            }
        }
    }

    public static void AddCoin()
    {
        coinCount++; // Add 1 coin to the coin count
        coinText.text = $" x{coinCount:00}"; // Update the coin display
        // This stays as Ox00 up until it hits Ox10 and so on
    }

    public static void CheckForCB(CharacterController controller, Transform playerTransform)
    {
        // Center of the Mario
        Vector3 playerCenter = playerTransform.TransformPoint(controller.center);
        
        // Distance from that center to a little above the Mario (needed for the following)
        float distance = (controller.height * 0.5f) + 0.1f;
        
        // Cast the ray directly upwards, check for coin block
        if (Physics.Raycast(playerCenter, Vector3.up, out RaycastHit hit, distance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Question"))
            {
                ScoreCounter.AddScore(100);
                AddCoin(); // Call function to add a coin
                
                Debug.Log("Question Block at Position: " + hit.transform.position + " was hit"); // Print the position of whatever question block was hit

                Destroy(hit.collider.transform.gameObject);
            }
        }
    }
}
