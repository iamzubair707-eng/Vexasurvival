using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    
    public Slider healthBar;
    public GameObject gameOverPanel;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        
        UpdateHealthBar();
        
        // Red flash effect
        StartCoroutine(FlashRed());
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        
        UpdateHealthBar();
    }
    
    void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;
    }
    
    void Die()
    {
        Debug.Log("Player Died!");
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        // Disable player movement
        this.enabled = false;
    }
    
    System.Collections.IEnumerator FlashRed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            sr.color = Color.white;
        }
    }
}