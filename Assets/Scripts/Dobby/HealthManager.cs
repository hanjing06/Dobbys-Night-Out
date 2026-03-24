using UnityEngine;

public class DobbyHealthManager : MonoBehaviour
{
    public int dobbyHealth = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeHealth(int num)
    {
        if (dobbyHealth > 0 && dobbyHealth < 100)
        {
            dobbyHealth += num;
            if (CheckIfLose() == true)
            {
                Destroy(gameObject);
            }
        } 
    }

    public bool CheckIfLose()
    {
        if (dobbyHealth <= 0)
        {
            return true;
        }

        return false;
    }
}
