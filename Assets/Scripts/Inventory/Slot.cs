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

    public void AddItem(string itemName, int amt, Sprite itemIcon)
    {
        this.itemName = itemName;
        this.amt = amt;
        this.itemIcon = itemIcon;
        isFull = true;
        
        //add the amount of item to text field and make it visible
        this.amtText.text = amt.ToString();
        amtText.enabled = true;
        
        this.itemImage.sprite = itemIcon;
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
        invManager.DeselectSlots();
        shade.SetActive(true);
        isSelected = true;
    }

    public void OnRightClick()
    {
        
    }
}
