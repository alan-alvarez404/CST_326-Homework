using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    
    public Vector3 offset = new Vector3(0f, 8f, -6f);
    
    public Vector3 lookOffset = new Vector3(0, 1.5f, 0f);

    void LateUpdate()
    {
        transform.position = target.position + offset;
        transform.LookAt(target.position + lookOffset);
    }

}
