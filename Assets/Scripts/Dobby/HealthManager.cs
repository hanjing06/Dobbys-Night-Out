using UnityEngine;
using UnityEngine.UI;

public class HealthManager: MonoBehaviour
{
    public int health;
    
    //health bar sprites
    public Sprite health100;
    public Sprite health80;
    public Sprite health60;
    public Sprite health40;
    public Sprite health20;

    public Image healthBar;

    public int maxHealth = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        healthBar.sprite = health100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddHealth(int num)
    {
        if (health < 100)
        {
            health += num;
            DisplayHealthBarSprite(health);
        }
    }

    public void TakeDamage(int num)
    {
        health -= num;
        DisplayHealthBarSprite(health);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void DisplayHealthBarSprite(int health)
    {
        if (health <= 100 && health > 80)
        {
            healthBar.sprite = health100;
        } else if (health <= 80 && health > 60)
        {
            healthBar.sprite = health80;
        } else if (health <= 60 && health > 40)
        {
            healthBar.sprite = health60;
        } else if (health <= 40 && health > 20)
        {
            healthBar.sprite = health40;
        } else
        {
            healthBar.sprite = health20;
        }
    }
}
