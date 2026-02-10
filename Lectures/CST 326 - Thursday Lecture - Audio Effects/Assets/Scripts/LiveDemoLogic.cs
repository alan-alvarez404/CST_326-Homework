using UnityEngine;
using UnityEngine.InputSystem;

public class LiveDemoLogic : MonoBehaviour
{
    public Transform carTransform;
    public LiveDemoAudioManager audioManager;

    private Vector3 startingPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = carTransform.position;
        audioManager.PlayCarEngineLoop();
    }

    // Update is called once per frame
    void Update()
    {
        // Car oscilaltes between starting position and some distance forward
        float displacementPercentage = Mathf.Sin(Time.timeSinceLevelLoad) + 0.5f + 0.5f;

        Vector3 displacement = carTransform.forward * 100f;
        
        Vector3 newPosition = startingPosition + displacementPercentage * displacement;
        carTransform.position = newPosition;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            audioManager.PlayCarHorn();
        }
        
    }
}
