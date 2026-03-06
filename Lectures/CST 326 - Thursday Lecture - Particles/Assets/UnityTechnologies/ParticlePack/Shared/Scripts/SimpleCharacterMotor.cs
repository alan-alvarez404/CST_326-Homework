using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleCharacterMotor : MonoBehaviour
{
    public CursorLockMode cursorLockMode = CursorLockMode.Locked;
    public bool cursorVisible = false;
    [Header("Movement")]
    public float walkSpeed = 2;
    public float runSpeed = 4;
    public float gravity = 9.8f;
    [Space]
    [Header("Look")]
    public Transform cameraPivot;
    public float lookSpeed = 45;
    public bool invertY = true;
    [Space]
    [Header("Smoothing")]
    public float movementAcceleration = 1;

    CharacterController controller;
    Vector3 movement, finalMovement;
    float speed;
    Quaternion targetRotation, targetPivotRotation;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = cursorLockMode;
        Cursor.visible = cursorVisible;
        targetRotation = targetPivotRotation = Quaternion.identity;
    }

    void Update()
    {
        UpdateTranslation();
        UpdateLookRotation();
    }

    void UpdateLookRotation()
    {
        var d = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;
        var x = d.y * 0.1f;
        var y = d.x * 0.1f;
        x *= invertY ? -1 : 1;
        targetRotation = transform.localRotation * Quaternion.AngleAxis(y * lookSpeed * Time.deltaTime, Vector3.up);
        targetPivotRotation = cameraPivot.localRotation * Quaternion.AngleAxis(x * lookSpeed * Time.deltaTime, Vector3.right);

        transform.localRotation = targetRotation;
        cameraPivot.localRotation = targetPivotRotation;
    }

    void UpdateTranslation()
    {
        if (controller.isGrounded)
        {
            var k = Keyboard.current;
            var x = k != null
                ? ((k.dKey.isPressed || k.rightArrowKey.isPressed) ? 1f : 0f) - ((k.aKey.isPressed || k.leftArrowKey.isPressed) ? 1f : 0f)
                : 0f;
            var z = k != null
                ? ((k.wKey.isPressed || k.upArrowKey.isPressed) ? 1f : 0f) - ((k.sKey.isPressed || k.downArrowKey.isPressed) ? 1f : 0f)
                : 0f;
            var run = k != null && k.leftShiftKey.isPressed;

            var translation = new Vector3(x, 0, z);
            speed = run ? runSpeed : walkSpeed;
            movement = transform.TransformDirection(translation * speed);
        }
        else
        {
            movement.y -= gravity * Time.deltaTime;
        }
        finalMovement = Vector3.Lerp(finalMovement, movement, Time.deltaTime * movementAcceleration);
        controller.Move(finalMovement * Time.deltaTime);
    }
}
