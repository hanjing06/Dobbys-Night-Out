using UnityEngine;

public class Item: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //the following line makes the following variable editable in the unity editor even if they are private
    [SerializeField] private string itemName;
    [SerializeField] private int amt;

    [SerializeField] private Sprite itemIcon;

    private InventoryManager inventoryManager;
    void Start()
    {
        //get the InventoryManager component of the InventoryCanvas
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inventoryManager.AddItem(itemName, amt, itemIcon);
            Destroy(gameObject);
        }
    }
}
