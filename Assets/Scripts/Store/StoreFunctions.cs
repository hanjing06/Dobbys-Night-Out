using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoreFunctions: MonoBehaviour, IPointerClickHandler
{
    private InventoryManager invManager;
    private CurrencyManager currencyManager;
    public GameObject store;
    public Sprite itemIcon;
    public int price;
    [TextArea] public string itemDescription;
    public GameObject completeUI;
    public TMP_Text UItext;
    private bool isActive;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = false;
        completeUI.SetActive(false);
        invManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        currencyManager = GameObject.Find("CurrencyCanvas").GetComponent<CurrencyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            completeUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isActive)
            {
                Time.timeScale = 1; //game moves at regular speed
                store.SetActive(false); //deactivates inventory
                isActive = false;
            }
            else
            {
                Time.timeScale = 0; //time is paused during active inventory
                store.SetActive(true); //activates inventory
                isActive = true;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (gameObject.CompareTag("buyButton"))
        {
            
        } 
        
    }

    public void buyYogurt()
    {
        if (currencyManager.numSpiders >= price)
        {
            Debug.Log("clicked yogurt");
            completeUI.SetActive(true);
            UItext.text = "A yogurt has been added to your inventory!";
            currencyManager.numSpiders -= price;
            invManager.AddItem("Yogurt", 1, itemIcon, itemDescription);
        }
        else
        {
            Debug.Log("not enough money");
            completeUI.SetActive(true);
            UItext.text = "You don't have enough spiders :(";
        }
    }
}
