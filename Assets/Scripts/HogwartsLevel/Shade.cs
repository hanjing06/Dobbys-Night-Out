using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * this is a class for the spaces in the cryptogram
 * they have a letter assigned to them to identify if the clicked key matches the letter in the space
 */
public class Shade: MonoBehaviour//, IPointerClickHandler
{
    [SerializeField] private char correspondingLetter;
    private CryptogramManager cryptogramManager;
    public GameObject shadeComponent;
    public bool isSelected;
    public Image shadeImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //shadeComponent.GetComponent<Image>().enabled = false;
        //get the image component of the shade
       // shadeComponent.SetActive(false);
        cryptogramManager = FindObjectOfType<CryptogramManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        
        Debug.Log("Space clicked: " + gameObject.name); // if this doesn't log, clicks aren't registering
            cryptogramManager.DeselectSlots();
            //Color c = shadeImage.color;
            //c.a = 1f;
            shadeComponent.SetActive(true);
            isSelected = true;
    }*/
    
   
    
   
    
}
