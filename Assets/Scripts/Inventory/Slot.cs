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
    public string itemDescription;
    public int amt;
    public Sprite itemIcon;
    public Sprite emptyIcon;
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
        //check if slot is full or not
        if (isFull)
        {
            return amt;
        }

        //update item variables
        this.itemName = itemName;
        this.itemIcon = itemIcon;
        this.itemDescription = itemDescription;
        this.itemImage.sprite = itemIcon;
        this.amt += amt;
        if (this.amt >= maxItems)
        {
            amtText.text = maxItems.ToString();
            amtText.enabled = true;
            isFull = true;
            
            //once a slot is full the extra items must be carries over
            int extraItems = this.amt - maxItems;
            this.amt = maxItems;
            return extraItems;
        }
        amtText.text = this.amt.ToString();
        amtText.enabled = true;
        return 0;
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        //using the selected item
        if (this.isSelected)
        {
            invManager.UseItem(this.itemName);
        }
        invManager.DeselectSlots();
        shade.SetActive(true);
        isSelected = true;
        itemNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemImage.sprite = itemIcon;
        if (itemImage.sprite == null)
        {
            itemImage.sprite = emptyIcon;
        }
    }

    public void OnRightClick()
    {
        
    }
}
