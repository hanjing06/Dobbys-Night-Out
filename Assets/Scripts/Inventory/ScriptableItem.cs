using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ScriptableItem", menuName = "Scriptable Objects/ScriptableItem")]
public class ScriptableItem: ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    public Sprite itemIcon;
    public Change change = new Change();
    public int changeAmt;

    public enum Change
    {
       None,
       AddHealth, //items with energy (ex: yogurt)
       TakeDamage, //harmful items in the environment (ex: tinfoil)
       Currency, //the spiders for purchasing from the store
       Power //power ups obtained from mystery box
    }

    public void UseItem()
    {
        if(change == Change.AddHealth)
        {
            GameObject.Find("HealthCanvas").GetComponent<HealthManager>().AddHealth(changeAmt);
        }
        if(change == Change.Currency)
        {
            GameObject.Find("CurrencyCanvas").GetComponent<CurrencyManager>().CollectCurrency(changeAmt);
        }

        if (change == Change.TakeDamage)
        {
            GameObject.Find("HealthCanvas").GetComponent<HealthManager>().TakeDamage(changeAmt);
        }

        if (change == Change.Power)
        {
            GameObject.Find("Player").GetComponent<AbilityManager>().EquipPowerUp();
        }
    }
}
