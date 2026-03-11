using UnityEngine;

// Keeps an object (like a name label) facing the camera.
public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;
    }
}
