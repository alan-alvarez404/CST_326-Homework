using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public AudioClip enemyTic;
    public AudioClip enemyTac;
    
    
    
    public delegate void EnemyDiedFunc(float points); // Func is delegate type
    public static event EnemyDiedFunc OnEnemyDied;
    
    // Static variables are associated with the object
    // It's associated with a class definition and not an instance
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Ouch!");
        
        // todo - destroy the bullet
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject); // this.gameObject does the same

            OnEnemyDied?.Invoke(10); // Question Mark = if null, don't
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
