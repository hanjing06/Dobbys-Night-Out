using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CryptogramManager: MonoBehaviour//, IPointerClickHandler
{
    //public bool open 
    public GameObject cryptogramUI;
    public bool isActive;
    private char selectedChar;
    public Letter[] lettersArr;
    public Shade[] shadeArr;
    public Letter[] letterKeyArr;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < lettersArr.Length; i++)
        {
            lettersArr[i].enabled = false;
            //shadeArr[i].gameObject.SetActive(false); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode pressed = GetInputKey();
        Debug.Log(pressed.ToString() + "was pressed");
        if (pressed != KeyCode.None)
        {
            RevealLetters(pressed);
        }
        
        if (isActive == true)
        {
            Time.timeScale = 0;  //time is paused 
            cryptogramUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;  //time is paused 
            cryptogramUI.SetActive(false);

        }
    }
    
    void RevealLetters(KeyCode input)
    {
        for (int i = 0; i < lettersArr.Length; i++)
        {
            for (int j = 0; j < letterKeyArr.Length; j++)
            {
                if (lettersArr[i].letter == (char)input)
                {
                    lettersArr[i].enabled = true;
                    if (lettersArr[i].letter == letterKeyArr[j].letter)
                    {
                        letterKeyArr[j].enabled = true;
                        letterKeyArr[j].correspondingLetter.text =  lettersArr[i].letter.ToString();
                    }
                }
            }
        }

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
 
    
    //DESELCTING SPACES
   
   
    
}
