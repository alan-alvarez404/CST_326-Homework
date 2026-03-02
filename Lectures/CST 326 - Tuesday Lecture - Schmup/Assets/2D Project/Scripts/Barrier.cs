using UnityEngine;
using Color = UnityEngine.Color;

public class Barrier : MonoBehaviour
{
    public int whichBarrier = 1;
    
    [Header("Health")]
    public int barrierHealth = 100;
    public int currentHealth;
    public int damagePerHit = 10;

    [Header("Color")] 
    public Color fullHealthColor = new Color(0.2f, 1f, 0.2f, 1f); // Neon green
    public Color lowHealthColor = new Color(0.05f, 0.2f, 0.05f, 1f); // Darker green\
    
    [Header("Size")] 
    public float fullHealthSize = 2.5f;
    public float sizeDecreasePerHit = 0.25f;
    public float lowHealthSize = 0.25f;
    
    private SpriteRenderer spriteRenderer;
    
    // Making a new delegate and event for when a barrier is broken
    public delegate void BarrierBroken(int whichBarrier);
    public static event BarrierBroken BarrierWasShattered;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = barrierHealth; // Spawn barrier with whatever health its given
        transform.localScale = Vector3.one * fullHealthSize;
        UpdateHealthColor();
        UpdateHealthSize();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Player Bullet") || collision.gameObject.layer == LayerMask.NameToLayer("Enemy Bullet")) && barrierHealth > 0)
        {
            currentHealth -= damagePerHit; // Take 10 points of damage
            
            Destroy(collision.gameObject); // Destroy the bullet
            
            UpdateHealthColor(); // Update the color relative to current health
            UpdateHealthSize(); // Update the size relative to current health
            
            if (currentHealth <= 0)
            {
                BarrierWasShattered?.Invoke(whichBarrier);
                Debug.Log($"Barrier number {whichBarrier} was shattered");
                Destroy(gameObject);
            }
        }
    }
    
    // Using Lerp to interpolate smoothly from the full health color to the lower health color
    private void UpdateHealthColor()
    {
        if (spriteRenderer == null) return;

        float t = 1f - (currentHealth / (float) barrierHealth); // 0 at full, 1 at empty
        spriteRenderer.color = Color.Lerp(fullHealthColor, lowHealthColor, t);
    }

    // Using this to go from full health size to the lower health size
    private void UpdateHealthSize()
    {
        if (spriteRenderer == null) return;

        int hitsTaken = (barrierHealth - currentHealth) / damagePerHit;
        
        float newSize = fullHealthSize - (hitsTaken * sizeDecreasePerHit);
        newSize = Mathf.Max(newSize, lowHealthSize);
        
        transform.localScale = new Vector3(newSize, newSize, 1f); // Not changing the z since it's a 2d sprite and that just wouldn't even be noticeable at this point
    }
}
