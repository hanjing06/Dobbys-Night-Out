using UnityEngine;

public class ContortionAbility : MonoBehaviour
{
    [Header("Unlock")]
    public bool isUnlocked = false;
    public int amtEquipped; //the number of times a certain power up is equipped from the inventory
    [SerializeField] private KeyCode useKey = KeyCode.KeypadEnter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUnlocked)
        {
            return;
        } 
        if (Input.GetKeyDown(useKey))
        {
            PerformContortion();
            
            //lock the ability if all equipped are used 
            amtEquipped--;
            if (amtEquipped <= 0)
            {
                LockAbility();
                Debug.Log("Muscle Up is unavailable");
            }
        }
    }
    
    public void UnlockAbility()
    {
        isUnlocked = true;
    }
    
    public void LockAbility()
    {
        isUnlocked = false;
    }

    public void PerformContortion()
    {
        //add power up functionality here
    }
}
