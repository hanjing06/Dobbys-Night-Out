using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PuzzleManager : MonoBehaviour
{
    public PuzzleSlot[] slots;
    public GameObject puzzle;
    public bool isActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(GetInputKey() + " was pressed");
        if (isActive)
        {
            Time.timeScale = 0;  //time is paused 
            puzzle.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;  //time is paused 
            puzzle.SetActive(false);

        }

        Exit();
    }
    
    //SELECTION METHODS
    public void DeselectPuzzleSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].shade.SetActive(false);
            slots[i].isSelected = false;
        }
    }
    
    //EXITING THE PUZZLE (WARNING YOU LOSE PROGRESS = INCREASED DIFFICULTY)
    void Exit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isActive = false;
        }
    }
}
