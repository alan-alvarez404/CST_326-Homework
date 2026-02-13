using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class DemoLogic : MonoBehaviour
{
    public Rigidbody payload;
    public float parachuteDeployHeight = 3f;
    public Transform debugSphere;
    public Camera rayCamera;
    public Transform parachute;

    float _startingDrag;

    IEnumerator LearnAboutCoroutines()
    {
        Debug.Log("starting coroutine #0");
        yield return new WaitForSeconds(1);
        Debug.Log("Waited #1");
        yield return new WaitForSeconds(1);
        Debug.Log("Waited #2");
    }
    
    IEnumerator AnimateParachuteScale(Vector3 startScale, Vector3 endScale, float duration)
    {
        // start the clock
        float timeElapsed = 0f;

        // loop
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float percentComplete =  timeElapsed / duration;
            // - each frame, adjust the scale by the percentage of time elapsed toward the duration
            parachute.localScale = Vector3.Lerp(startScale, endScale, percentComplete);
                
            yield return null;
        }
            
        // clamp the final scale to be the end scale
        parachute.localScale = endScale;
    }
    
    void Start()
    {
        _startingDrag = payload.linearDamping;
        // StartCoroutine(LearnAboutCoroutines());
        StartCoroutine(AnimateParachuteScale(Vector3.zero, Vector3.one, 0.1f));
    }
    
    // Update is called once per frame
    void Update()
    {
        // Create ray from object downward x amount
        Ray proximityRay = new Ray(payload.position + Vector3.up * 0.01f, Vector3.down);

        // test if ray hits something
        bool intersects = Physics.Raycast(proximityRay, out RaycastHit hitInfo);

        // - if it does, make the visual red, OR blue
        if (intersects &&
            hitInfo.distance <
            parachuteDeployHeight) // Have to check a distance or else the ray's detection distance will be infinite
        {
            parachute.gameObject.SetActive(true);
            payload.linearDamping = 7f;
            Debug.DrawRay(proximityRay.origin, proximityRay.direction, Color.red);
        }
        else
        {
            parachute.gameObject.SetActive(false);
            payload.linearDamping = _startingDrag;
            Debug.DrawRay(proximityRay.origin, proximityRay.direction, Color.blue);
        }

        Vector3 mousePosition = Mouse.current.position.value;
        Ray screenRay = rayCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(screenRay, out RaycastHit screenHitInfo))
        {
            Debug.DrawRay(screenRay.origin, screenRay.direction, Color.blueViolet);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                debugSphere.position = screenHitInfo.point;
            }
        }
        
        
        
        float easeOutElastic(float x) {
            float c4 = (2 * Mathf.PI) / 3;

            return x == 0
                ? 0
                : x == 1
                    ? 1
                    : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75) * c4) + 1;
        } 
    }
}
