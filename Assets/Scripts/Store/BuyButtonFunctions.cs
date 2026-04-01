using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuyButtonFunctions: MonoBehaviour, IPointerClickHandler
{
    private InventoryManager invManager;
    public ScriptableItem itemToBuy;
    public Sprite yogurtImage;
    public Sprite boxImage;
    private ScriptableItem mysteryAbility;
    private Canvas popUp;
    public TMP_Text itemLabel;
    private Canvas backgroundShade;
    private AbilityManager abilityManager;
    public Image popUpImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        invManager = GameObject.Find("StoreCanvas").GetComponent<InventoryManager>();
        abilityManager = FindObjectOfType<AbilityManager>();
        popUp = GameObject.Find("OpenMysteryCanvas").GetComponent<Canvas>();
        backgroundShade = GameObject.Find("ShadeCanvas").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            popUp.enabled = false;
            backgroundShade.enabled = false;
            itemLabel.text = "";
            popUpImage.sprite = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (gameObject.CompareTag("buyMystery"))
        {
            Debug.Log("clicked mystery");
            popUp.enabled = true;
            popUpImage.sprite = boxImage;
            backgroundShade.enabled = true;
            mysteryAbility = abilityManager.mysteryAbility;
            itemLabel.text = "You have unlocked an ability.";
            invManager.AddItem(mysteryAbility.itemName, 1, mysteryAbility.itemIcon, mysteryAbility.itemDescription);

        } 
        if (gameObject.CompareTag("buyYogurt"))
        {
            Debug.Log("clicked yogurt");
            popUp.enabled = true;
            popUpImage.sprite = yogurtImage;
            backgroundShade.enabled = true;
            itemLabel.text = "A yogurt has been added to your inventory.";
            invManager.AddItem(itemToBuy.itemName, 1, itemToBuy.itemIcon, itemToBuy.itemDescription);
           
        } 
        
    }
}
