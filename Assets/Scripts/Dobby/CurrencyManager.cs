using UnityEngine;
using TMPro;

public class CurrencyManager: MonoBehaviour
{
    public int numSpiders; //in this game the spiders are the currency
    public TMP_Text currencyDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numSpiders = 0; //always start off with 0
        UpdateCurrency();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrency();
    }

    public void CollectCurrency(int num)
    {
        numSpiders += num;
        UpdateCurrency();
    }

    public void PurchaseItem(int price, string itemName, int amt, Sprite itemIcon, string itemDescription)
    {
        //subtract from currency and add purchased item to the inventory
        numSpiders -= price;
        UpdateCurrency();
        GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>().AddItem(itemName, amt, itemIcon, itemDescription);
    }

    public void UpdateCurrency()
    {
        currencyDisplay.text = numSpiders.ToString();
    }
}
