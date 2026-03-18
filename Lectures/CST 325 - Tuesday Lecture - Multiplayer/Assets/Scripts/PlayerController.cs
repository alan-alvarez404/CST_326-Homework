using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private float velocityY = 0f;
    private bool isGrounded = true;

    private const float gravity = -9.8f;
    private const float jumpForce = 5f;
    
    //Vector3 moveDirection = new Vector3(0f, 0f, 0f);
    
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocityY = jumpForce;
            isGrounded = false;
        }
        velocityY += gravity * Time.deltaTime;

        transform.Translate(new Vector3(h * speed * Time.deltaTime, velocityY * Time.deltaTime, v * speed * Time.deltaTime), Space.World);

        if (transform.position.y <= 0.5f)
        {
            transform.position = new Vector3( transform.position.x, 0.5f, transform.position.z);
            isGrounded = true;
        }
        
        /*
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        */

        if (transform.position.x < -5f || transform.position.x > 5f)
        {
            transform.position = Vector3.zero;
            //transform.rotation = Quaternion.identity;
        }
    }
}
