using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;

        [TextArea(2, 5)]
        public string dialogueText;

        public Sprite characterSprite;
    }

    [Header("Timeline")]
    public PlayableDirector timelineDirector;
    public float startDialogueTime = 8f;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public Image characterImage;
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public Button closeButton;

    [Header("Dialogue Lines")]
    public DialogueLine[] lines;

    [Header("Scene Transition")]
    public string nextSceneName;

    private int currentLineIndex = 0;
    private bool dialogueStarted = false;
    private bool dialogueActive = false;
    private int startFrame = -1;

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(EndDialogueAndLoadNextScene);
        }
    }

    void Update()
    {
        CheckTimelineStart();

        if (!dialogueActive)
            return;

        // Prevent the same click/frame that opened the dialogue from skipping instantly
        if (Time.frameCount == startFrame)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextDialogue();
        }
    }

    void CheckTimelineStart()
    {
        if (dialogueStarted)
            return;

        if (timelineDirector == null)
            return;

        if (timelineDirector.state == PlayState.Playing && timelineDirector.time >= startDialogueTime)
        {
            StartDialogue();
        }
    }

    public void StartDialogue()
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines assigned.");
            return;
        }

        dialogueStarted = true;
        dialogueActive = true;
        currentLineIndex = 0;
        startFrame = Time.frameCount;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (currentLineIndex < 0 || currentLineIndex >= lines.Length)
            return;

        DialogueLine line = lines[currentLineIndex];

        if (characterNameText != null)
            characterNameText.text = line.characterName;

        if (dialogueText != null)
            dialogueText.text = line.dialogueText;

        if (characterImage != null)
            characterImage.sprite = line.characterSprite;
    }

    public void NextDialogue()
    {
        currentLineIndex++;

        if (currentLineIndex >= lines.Length)
        {
            EndDialogueAndLoadNextScene();
            return;
        }

        ShowCurrentLine();
    }

    public void EndDialogueAndLoadNextScene()
    {
        dialogueActive = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is empty. Assign it in the Inspector.");
        }
    }
}