using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScroll : MonoBehaviour
{
    public float leftMostX = 13.25f; // Left Edge of the Level
    public float rightMostX = 211.75f; // Right Edge of the Level
    public Camera mainCamera;
    public float cameraMoveSpeed = 10f;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Assign the main camera if not assigned yet
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
        // This script is part of the extra credit where you can make the camera move when pressing
        // the left or right cursor keys. Except I'm doing it so the left and right arrow keys move
        // the camera rather than controlling with the mouse.
        // This is because you have to click the bricks or question block to destroy the bricks or
        // gain more coins. But if I map them to the left or right cursor keys it'll move the camera
        // as you try to collect coins from the question block or try to destroy a brick block.
        float move = 0f;

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            move -= 1f;
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            move += 1f;
        }

        // So that you can tell via the console if the keys are being pressed or not
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            Debug.Log("Left Arrow Key Pressed. Camera Moving Left");
        }
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            Debug.Log("Right Arrow Key Pressed. Camera Moving Right");
        }        
        
        if (move == 0f)
        {
            return;
        }

        Vector3 pos = transform.position;
        pos.x += move * cameraMoveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, leftMostX, rightMostX);

        transform.position = pos;
    }
}
