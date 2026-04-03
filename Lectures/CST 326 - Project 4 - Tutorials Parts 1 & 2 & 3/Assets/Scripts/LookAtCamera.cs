using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    // Basically created a drop down menu inside the ProgressBarUI of the CuttingCounter with four options
    private enum Mode
    {
        LookAt, 
        LookAtInverted,
        CameraForward,
        CameraForwardInverted,
    }
    
    [SerializeField] private Mode mode;
    
    // Runs after regular Update()
    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCaamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCaamera);
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = - Camera.main.transform.forward;
                break;
        }
    }
    
    
}
