using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
	this script handles the following...
		i. opens the inventory when the corresponding button is pressed
	   ii. pauses the game while the inventory is open
*/

public class InventoryManager: MonoBehaviour
{
	public GameObject inventory;
	private bool isActive;
	public Slot[] slot;
	public ScriptableItem[]  items;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		//note: 'Inventory' is a seperately created input asset that toggles the inventory when the 'I' key is pressed
        if(Input.GetKeyDown(KeyCode.I) && isActive)
        {
	        Time.timeScale = 1; //game moves at regular speed
			inventory.SetActive(false); //deactivates inventory
			isActive = false;
		} else if(Input.GetKeyDown(KeyCode.I) && !isActive)
        {
	        Time.timeScale = 0;  //time is paused during active inventory
			inventory.SetActive(true); //activates inventory
			isActive = true;
		}
		
    }

    public void UseItem(string itemName)
    {
	    for (int i = 0; i < items.Length; i++)
	    {
		    if (items[i].itemName == itemName)
		    {
			    items[i].UseItem();
		    }
	    }
    }

    public int AddItem(string itemName, int amt, Sprite itemIcon, string itemDescription)
    {
	    Debug.Log(itemName + "(" + amt + ") has been added to the inventory."); //print line to keep track of whats in the inventory
	    
	    //use recursion to find the first free slot to add item
	    for (int i = 0; i < slot.Length; i++)
	    {
		    //store an item if the slot is empty and allows for stacking if collecting multiple of same item
		    if (slot[i].isFull == false && slot[i].name == itemName || slot[i].amt == 0)
		    {
			    int itemsLeft = slot[i].AddItem(itemName, amt, itemIcon, itemDescription);
			    if (itemsLeft > 0) {
				    itemsLeft = slot[i].AddItem(itemName, amt, itemIcon, itemDescription);
			    }

			    return itemsLeft;
		    }
	    }
	    return amt;
    }

    public void DeselectSlots()
    {
	    for (int i = 0; i < slot.Length; i++)
	    {
		    slot[i].shade.SetActive(false);
		    slot[i].isSelected = false;
	    }
    }
}
