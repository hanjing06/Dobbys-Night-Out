using UnityEngine;

public class AbilityManager: MonoBehaviour
{
    //ability references
    private GameObject player;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //method for randomizing a power up after using mystery box
    public void EquipPowerUp()
    {
        //choose a random number from 1-5 inclusive
        int rand = Random.Range(1, 6);
        
        //switch-case for corresponding with random number
        switch (rand)
        {
            case 1: //cat smack
                player.GetComponent<CatSmackAbility>().UnlockAbility();
                player.GetComponent<CatSmackAbility>().amtEquipped++;
                break;
            case 2: //muscle up
                player.GetComponent<MuscleUpAbility>().UnlockAbility();
                player.GetComponent<MuscleUpAbility>().amtEquipped++;
                break;
            case 3: //contortion
                player.GetComponent<ContortionAbility>().UnlockAbility();
                player.GetComponent<ContortionAbility>().amtEquipped++;
                break;
            case 4: //high jump
                player.GetComponent<HighJumpAbility>().UnlockAbility();
                player.GetComponent<HighJumpAbility>().amtEquipped++;
                break;
            case 5: //throw yogurt
                player.GetComponent<ThrowYogurtAbility>().UnlockAbility();
                player.GetComponent<ThrowYogurtAbility>().amtEquipped++;
                break;
        }

    }
}
