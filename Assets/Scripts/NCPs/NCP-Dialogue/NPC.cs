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
        Debug.Log("Interact called on NPC");

        if (dialogueData == null)
        {
            Debug.LogWarning("No dialogue assigned!");
            return;
        }

        if (isdialogueActive)
        {
            Debug.Log("Dialogue already active, going to next line");
            nextLine();
        }
        else
        {
            Debug.Log("Starting dialogue now");
            StartDialogue();
        }
    }

    public bool CanInteract()
    {
        return !isdialogueActive;
    }

    void StartDialogue()
    {
        Debug.Log("StartDialogue entered");

        isdialogueActive = true;
        dialogueIndex = 0;

        if (nameText != null)
            nameText.SetText(dialogueData.npcName);
        else
            Debug.LogWarning("nameText is null");

        if (portraitImage != null)
            portraitImage.sprite = dialogueData.npcPortrait;
        else
            Debug.LogWarning("portraitImage is null");

        if (dialoguePanel != null)
        {
            Debug.Log("Setting dialogue panel active");
            dialoguePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("dialoguePanel is null");
        }

        StartCoroutine(TypeLine());
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
