using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class AgentController : MonoBehaviour
{
    public enum MouseButton
    {
        Left,
        Right,
    }
    
    public Transform destinationMarker;
    public MouseButton mouseButton;
    
    
    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    
    // Update is called once per frame
    void Update()
    {
        ButtonControl buttonControl = (mouseButton == MouseButton.Left) ? Mouse.current.leftButton : Mouse.current.rightButton;

        if (buttonControl.wasPressedThisFrame)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.value); // Auto convert to Vector3
            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo))
            {
                destinationMarker.position = hitInfo.point;
                _agent.SetDestination(hitInfo.point);
            
            }
        }

    }
}
