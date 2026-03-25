using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpen { get; private set; }
    public string ChestID { get; private set; }

    public GameObject itemPrefab;
    public Sprite OpenSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);// gives the chest a new id if its null "??=" checks if the value is equal to null and if it is it assigns a new value
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanInteract()
    {
        return !IsOpen; // you can only interact if the chest is not open
    }

    public void Interact()
    {
        Debug.Log("Interact called. IsOpen BEFORE: " + IsOpen);
        if (!CanInteract()) return;

        SetOpened(true);
        DropItem(); // spawn item
        Debug.Log("IsOpen AFTER: " + IsOpen);
    }

    private void DropItem()
    {
        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position, transform.rotation); //
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpen = opened;

        if (IsOpen)
        {
            GetComponent<SpriteRenderer>().sprite = OpenSprite;
        }
    }
}
