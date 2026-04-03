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
            if (GetInputKey() != KeyCode.None)
            {
                Debug.Log(GetInputKey() + "was pressed");
            }
            if (string.Equals(GetInputKey().ToString(), letter, System.StringComparison.OrdinalIgnoreCase))
            {
                letterText.SetActive(true);
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
