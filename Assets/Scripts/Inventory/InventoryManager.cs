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

    public void AddItem(string itemName, int amt, Sprite itemIcon)
    {
	    Debug.Log(itemName + "(" + amt + ") has been added to the inventory."); //print line to keep track of whats in the inventory
	    
	    //use recursion to find the first free slot to add item
	    for (int i = 0; i < slot.Length; i++)
	    {
		    if(slot[i].isFull == false)
			{
				slot[i].AddItem(itemName, amt, itemIcon);
				return;//end recursive loop when empty slot is found
			}
	    }

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
