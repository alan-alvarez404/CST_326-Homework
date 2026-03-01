using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public AudioClip enemyTic;
    public AudioClip enemyTac;

    public int enemyType;
    
    public delegate void EnemyDiedFunc(float points); // Func is delegate type
    public static event EnemyDiedFunc OnEnemyDied;
    
    // Static variables are associated with the object
    // It's associated with a class definition and not an instance

    void Start()
    {
        // TODO: Make it so that the type of the enemy changes based on the tag given
        if (gameObject.tag == "30 Points")
        {
            enemyType = 3;
        } else if (gameObject.tag == "20 Points")
        {
            enemyType = 2;
        }
        else // 10 Point Enemy
        {
            enemyType = 1;
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ouch!");
        
        // todo - destroy the bullet
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject); // this.gameObject does the same

            if (gameObject.tag == "30 Points")
            {
                OnEnemyDied?.Invoke(30); // 30 Point Enemy
            } else if (gameObject.tag == "20 Points")
            {
                OnEnemyDied?.Invoke(20); // 20 Point Enemy
            }
            else // 10 Point Enemy
            {
                OnEnemyDied?.Invoke(10); // Question Mark = if null, don't
            }
        }
        
        // todo - trigger death animation
    }

    public void PlayTicSound()
    {
        GetComponent<AudioSource>().PlayOneShot(enemyTic);
        
        Debug.Log("Tic");
    }
    
    public void PlayTacSound()
    {
        GetComponent<AudioSource>().PlayOneShot(enemyTac);

        Debug.Log("Tac");
    }
}
