using UnityEngine;

public class LiveDemo : MonoBehaviour
{
    public float yawDegreesPerSecond = 45f;   
    public float yawOrbitPerSecond = 50f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hello World!");
    }

    // Update is called once per frame
    void Update()
    {
        Transform myTransform = GetComponent<Transform>();
        myTransform.Rotate(new Vector3(0f, yawDegreesPerSecond * Time.deltaTime, 0f));
        
        // Since the sun is at the origin, the planets should be able to orbit the origin
        transform.RotateAround(Vector3.zero, Vector3.up, yawOrbitPerSecond * Time.deltaTime);
    }
}
