using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    public int enemyType = 1;
    
    private Animator animator;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        ApplyEnemyType();
    }

    // Going to be called from SceneLoader.cs when the main menu is loaded
    public void SetEnemyType(int type) 
    {
        enemyType = type;
        ApplyEnemyType();
    }
    
    // A slightly modified version of the original SetType function
    public void ApplyEnemyType()
    {
        // Set the value in the animator so that the enemy iterates througth the right sprites
        if (animator != null)
        {
            animator.SetFloat("Enemy Type", enemyType); // The value in the animator has to be a float but we can still pass in an integer value and it'll work
        }
    }
}
