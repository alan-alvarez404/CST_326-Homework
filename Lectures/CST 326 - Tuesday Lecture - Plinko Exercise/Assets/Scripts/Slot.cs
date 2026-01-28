using UnityEngine;

public class Slot : MonoBehaviour
{
    public int slotNumber;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Slot {slotNumber} collided with {other.gameObject.name}");
    } 
    
    
}
