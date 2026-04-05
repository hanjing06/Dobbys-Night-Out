using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Cinemachine;

public class PuzzleSlot : MonoBehaviour, IPointerClickHandler
{
    public string letter;
    private PuzzleManager puzzleManager;
    public GameObject shade;
    public GameObject letterText;
    public bool isSelected;
    public bool letterVisible;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        puzzleManager = GameObject.Find("PuzzleCanvas").GetComponent<PuzzleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            KeyCode currentKey = GetInputKey();
            if (currentKey != KeyCode.None)
            {
                Debug.Log(currentKey + "was pressed");
                //updating health based on user input

                //ignore if mouse is clicked
                if (currentKey == KeyCode.Mouse0 || currentKey == KeyCode.Mouse1 || currentKey == KeyCode.Mouse2)
                {
                    return;
                } else if (string.Equals(currentKey.ToString(), letter, System.StringComparison.OrdinalIgnoreCase))
                {
                    //when the correct letter is pressed
                    letterText.SetActive(true);
                    letterVisible = true;
                    isSelected = false;
                    shade.SetActive(false);
                } else
                {
                    //when incorrect letter is pressed -- deal damage
                    puzzleManager.healthManager.TakeDamage(20);
                }
            }
           
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        puzzleManager.DeselectPuzzleSlot();
        shade.SetActive(true);
        isSelected = true;

    }
    
    KeyCode GetInputKey()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    return code;
                    
                }
            }
        }
        return KeyCode.None;
    }
    
}
