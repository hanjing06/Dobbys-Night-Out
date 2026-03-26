using UnityEngine;

public class SpiderControl : MonoBehaviour
{
    private CurrencyManager currencyManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currencyManager = GameObject.Find("Player").GetComponent<CurrencyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionTrigger2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Destroy(gameObject);
        }
    }
}
