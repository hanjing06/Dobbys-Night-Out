using UnityEngine;

public class SpiderControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            //numSpiders++;
            //Debug.Log("Spider touched! Total: " + numSpiders);
            Destroy(gameObject);
        }
    }
}
