using UnityEngine;
using UnityEngine.UI;

public class ObjectManager: MonoBehaviour
{
    //attach the scriptable object here
    public ScriptableItem objectData;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject.Find("CollectablePrefab").GetComponent<SpriteRenderer>().sprite = objectData.itemIcon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
