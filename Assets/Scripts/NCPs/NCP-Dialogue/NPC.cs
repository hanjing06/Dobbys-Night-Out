using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText,nameText;
    public Image portraitImage;

    private int dialogueIndex;
    private bool isTyping, isdialogueActive;

    public void Interact()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning("No dialogue assigned!");
            return;
        }

        if (isdialogueActive)
        {
            nextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    public bool CanInteract()
    {
        return !isdialogueActive;
    }

    void StartDialogue()
    {
        Debug.Log("Starting dialogue with " + dialogueData.npcName);
        isdialogueActive = true;
        dialogueIndex = 0;
        
        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
        //typeLine
    }

    void nextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");
        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        isTyping = false;
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressSpeed);
            nextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isdialogueActive = false;
        dialoguePanel.SetActive(false);
        dialogueText.SetText("");
    }
}
