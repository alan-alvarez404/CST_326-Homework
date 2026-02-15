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
                    Destroy(hit.collider.gameObject); // Destroy any object tagged Brick if ray intersects with it
                    Debug.Log("Brick at Position: " + hit.point + " was destroyed");
                }
            }
        }
        
    }

}
