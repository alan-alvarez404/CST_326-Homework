using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    // False if from player, True if from enemy
    public bool isEnemyBullet = false;

    public static int MaxPlayerBullets = 1;
    public static int MaxEnemyBullets = 3;
    
    private static int playerBulletsActive = 0;
    private static int enemyBulletsActive = 0;

    private bool created = false;
    
    public float speed = 5;
    public float duration = 1.67f; // 1.67 is good
    
    private Rigidbody2D rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        // Based on what type of bullet it is it'll go up for player down for enemy
        rigidBody.linearVelocity = (isEnemyBullet ? Vector2.down : Vector2.up) * speed;
        Debug.Log("Wwweeeeee");

        if (duration > 0f)
        {
            Destroy(gameObject, duration);
        }
    }

    void OnDestroy()
    {
        if (!created)
        {
            return;
        }

        if (isEnemyBullet)
        {
            enemyBulletsActive = Mathf.Max(0, enemyBulletsActive - 1);
        }
        else
        {
            playerBulletsActive = Mathf.Max(0, playerBulletsActive - 1);
        }
        
        created = false;
    }
    

    // Will be called to shoot a bullet
    public static bool ShootBullet(GameObject bulletPrefab, Vector3 position, bool isEnemyBullet)
    {
        if (bulletPrefab == null) return false;

        if (isEnemyBullet)
        {
            if (enemyBulletsActive >= MaxEnemyBullets)
            {
                return false; // If attempting to have more bullets than the limit
            }

            enemyBulletsActive++;
        }
        else // Player Bullets
        {
            if (playerBulletsActive >= MaxPlayerBullets)
            {
                return false;
            }

            playerBulletsActive++;
        }

        GameObject bulletInstance = Instantiate(bulletPrefab, position, Quaternion.identity);

        Bullet bullet = bulletInstance.GetComponent<Bullet>();

        bullet.isEnemyBullet = isEnemyBullet;
        
        // Give them the right tag if they're an enemy bullet or not
        bulletInstance.tag = isEnemyBullet ? "Enemy Bullet" : "Player Bullet";
        
        bullet.created = true;
        return true;
    }
}
