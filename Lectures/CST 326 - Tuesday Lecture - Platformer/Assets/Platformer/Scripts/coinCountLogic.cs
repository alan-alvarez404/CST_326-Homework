using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CoinCountLogic : MonoBehaviour
{
    public Camera rayCamera;
    public int coinCount;
    public TextMeshProUGUI coinCountText;
    
    void Start()
    {
        // Set the initial coin counter to the proper Ox00 count
        coinCount = 0;
        coinCountText.text = $"Ox{coinCount:00}";
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
}
