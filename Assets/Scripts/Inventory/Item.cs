using UnityEngine;

public class Item: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //the following line makes the following variable editable in the unity editor even if they are private
    [SerializeField] private ScriptableItem itemInfo;
    [SerializeField] private int amt;

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
            int itemsLeft = inventoryManager.AddItem(itemInfo.itemName, amt, itemInfo.itemIcon, itemInfo.itemDescription);
            if (itemsLeft <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                amt = itemsLeft;
            }
        }
    }
}
