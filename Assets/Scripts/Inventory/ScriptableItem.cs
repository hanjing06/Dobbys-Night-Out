using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableItem", menuName = "Scriptable Objects/ScriptableItem")]
public class ScriptableItem: ScriptableObject
{
    public string itemName;
    public Change change = new Change();
    public int changeAmt;

    public enum Change
    {
       none,
       health,
       currency
    }

    public void UseItem()
    {
        if(change == Change.health)
        {
            GameObject.Find("DobbyHealthManager").GetComponent<DobbyHealth>().ChangeHealth(changeAmt);
        }
        if(change == Change.currency)
        {
            GameObject.Find("CurrencyManager").GetComponent<numSpiders>().collectCurrency(changeAmt);
        }
    }
}
