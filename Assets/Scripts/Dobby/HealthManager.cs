using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthManager: MonoBehaviour
{
    public int health;
    
    //health bar sprites
    public Sprite health100;
    public Sprite health80;
    public Sprite health60;
    public Sprite health40;
    public Sprite health20;
    
    //handling death depending on level
    public bool restartOnDeath;
    public string restartScene = "";
    public GameObject lossUI;
    public bool dead = false;

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
        } else if (health == 100)
        {
            Debug.Log("cannot use item");
        }
    }

    public void TakeDamage(int num)
    {
        health -= num;
        if (health <= 0)
        {
            health = 0;
            DisplayHealthBarSprite(health);
            HandleDeath();
        } else
        {
            DisplayHealthBarSprite(health);
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
    
    //this method determines what happens when the health reaches 0 or below
    void HandleDeath()
    {
        Time.timeScale = 1;
        if (restartOnDeath)
        {
            if (restartScene != "")
            {
                StartCoroutine(ShowLossMessage());
            }
        }
        else
        {
            //other option if not restarting level (add conditions if you want something different to happen in your level)
            Destroy(gameObject);
        }
    }

    IEnumerator ShowLossMessage()
    {
        dead = true;
        lossUI.SetActive(true);
        //display the message until the user presses r key
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));

        if (restartScene != "")
        {
            SceneManager.LoadScene(restartScene);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}
