using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HotSwapper : MonoBehaviour
{
    public GameObject usedQuestionBlockPrefab; // Prefab that will be swapped in

    public static GameObject swappablePrefab;
    
    void Awake()
    {
        swappablePrefab = usedQuestionBlockPrefab;
    }
    
    // Question block is hit = pass in question block and its tag
    public static void SwapOutBlock(GameObject hitObject, string currentTag)
    {
        if (hitObject == null) return; // No object given

        if (currentTag == "Question")
        {
            Transform transform = hitObject.transform;
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            
            Destroy(hitObject); // Destroy the original question block
            Instantiate(swappablePrefab, position, rotation);
        }
    }
}
