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

    void Awake()
    {

    }
    
    // Going to be called from LevelParser.cs when the enemy gets instantiated
    public void SetType(int type)
    {
        enemyType = type;
        // Set the value in the animator so that the enemy iterates througth the right sprites
        GetComponent<Animator>().SetInteger("Enemy Type", enemyType);    
        // Just checking
        Debug.Log($"[{name}] SetType -> {enemyType} | Animator param now = {GetComponent<Animator>().GetInteger("Enemy Type")}");
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
