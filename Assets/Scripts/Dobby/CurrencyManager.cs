using UnityEngine;

public class CurrencyManager: MonoBehaviour
{
    public int numSpiders; //in this game the spiders are the currency
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numSpiders = 0; //always start off with 0
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollectCurrency(int num)
    {
        numSpiders += num;
    }

    public void PurchaseItem(int price, string itemName, int amt, Sprite itemIcon, string itemDescription)
    {
        //subtract from currency and add purchased item to the inventory
        numSpiders -= price;
        GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>().AddItem(itemName, amt, itemIcon, itemDescription);
    }
}
