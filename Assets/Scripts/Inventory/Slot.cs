using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

//IPointerClickHandler --> detects when the pointer clicks on it
public class Slot: MonoBehaviour, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    //item information...
    public string itemName;
    public int amt;
    public Sprite itemIcon;
    public bool isFull;
    
    //for slot...
    [SerializeField] private TMP_Text amtText;
    [SerializeField] private Image itemImage;
    [SerializeField] private int maxItems;
    
    //for item description...
    public TMP_Text itemDescriptionText; //text UI for the item description
    public TMP_Text itemNameText; //text UI for the item name
    public Image itemIconUI; //image UI for the item icon/sprite
    
    //variables for selecting items
    public GameObject shade;
    public bool isSelected;
    
    private InventoryManager invManager;
    void Start()
    {
        invManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int AddItem(string itemName, int amt, Sprite itemIcon, string itemDescription)
    {
        Debug.Log("amtText is null: " + (amtText == null) + " on slot: " + gameObject.name);
        //check if slot is full or not
        if (isFull)
        {
            return amt;
        }

        //update item variables
        this.itemName = itemName;
        this.itemIcon = itemIcon;
        itemImage.sprite = itemIcon;
        itemImage.enabled = true;
        this.amt += amt;
        
        //enable amount text 
        amtText.gameObject.SetActive(true);
        amtText.text = this.amt.ToString();
            
        //if a slot has the max number of items move to next available slot
        if (this.amt >= maxItems)
        {
            amtText.text = maxItems.ToString();
            amtText.enabled = true;
            isFull = true;
            
            int extraItems = this.amt - maxItems;
            this.amt = maxItems;
            return extraItems;
        }
        return 0;
    }

    public bool SlotHasItem()
    {
        return amt > 0 && !string.IsNullOrEmpty(itemName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        //using the selected item
        if (this.isSelected)
        {
            invManager.UseItem(this.itemName);
            UpdateSlot();
        }
        
        //ensure only one slot is selected at a time
        invManager.DeselectSlots();
        shade.SetActive(true);
        isSelected = true;
        UpdateItemViewer();

    }


    public void UpdateSlot()
    {
        //adjusts the slot based on the players actions
        if (SlotHasItem())
        {
            amt -= 1;
            itemImage.sprite = itemIcon;
            itemImage.enabled = true;
            amtText.gameObject.SetActive(true);
            amtText.text = amt.ToString();
        }
        else
        {
            itemImage.sprite =  null;
            itemImage.enabled = false;
            amtText.gameObject.SetActive(false);
            itemName = "";
            itemIcon = null;
            isFull = false;
        }
    }

    public void UpdateItemViewer()
    {
        ScriptableItem itemInfo = invManager.GetItem(itemName);
        //display info in the viewer
        if (SlotHasItem())
        {
            itemNameText.text = itemInfo.itemName;
            itemDescriptionText.text = itemInfo.itemDescription;
            itemIconUI.sprite = itemInfo.itemIcon;
            itemIconUI.GetComponent<Image>().enabled = true;
        }
        else
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";
            itemIconUI.sprite = null;
            itemIconUI.GetComponent<Image>().enabled = false;
        }
    }


}
