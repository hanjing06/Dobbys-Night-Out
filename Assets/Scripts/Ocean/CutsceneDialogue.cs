using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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
    public Animator dialogueAnimator;
    public Image characterImage;
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public Button closeButton;

    [Header("Dialogue Lines")]
    public DialogueLine[] lines;

    [Header("Scene Transition")]
    public string nextSceneName;

    [Header("Pause Gameplay")]
    public MonoBehaviour playerMovementScript;
    public Rigidbody2D playerRigidbody2D;
    public Animator playerAnimator;
    public bool pauseGameWithTimeScale = true;

    [Header("Animation Settings")]
    public float closeAnimationDuration = 0.5f;

    private int currentLineIndex = 0;
    private bool dialogueStarted = false;
    private bool dialogueActive = false;
    private bool isClosing = false;
    private int startFrame = -1;

    private Vector2 savedVelocity2D;
    private float savedAnimatorSpeed = 1f;

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(EndDialogueAndLoadNextScene);
        }

        // Very important:
        // Set the dialogue UI Animator to Unscaled Time in the Inspector too.
        // This lets the slide animation still play while the game is paused.
    }

    void Update()
    {
        CheckTimelineStart();

        if (!dialogueActive || isClosing)
            return;

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
        PauseGameplay();

        if (dialogueAnimator != null)
            dialogueAnimator.SetTrigger("Open");
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

    void PauseGameplay()
    {
        // Disable player input / movement script
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Freeze Rigidbody2D movement
        if (playerRigidbody2D != null)
        {
            savedVelocity2D = playerRigidbody2D.linearVelocity;
            playerRigidbody2D.linearVelocity = Vector2.zero;
            playerRigidbody2D.angularVelocity = 0f;
            playerRigidbody2D.simulated = false;
        }

        // Keep the player's current sprite visible by freezing the animator
        if (playerAnimator != null)
        {
            savedAnimatorSpeed = playerAnimator.speed;
            playerAnimator.speed = 0f;
        }

        // Pause the rest of the game
        if (pauseGameWithTimeScale)
            Time.timeScale = 0f;
    }

    void ResumeGameplay()
    {
        if (pauseGameWithTimeScale)
            Time.timeScale = 1f;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (playerRigidbody2D != null)
        {
            playerRigidbody2D.simulated = true;
            playerRigidbody2D.linearVelocity = Vector2.zero;
            playerRigidbody2D.angularVelocity = 0f;
        }

        if (playerAnimator != null)
            playerAnimator.speed = savedAnimatorSpeed;
    }

    public void EndDialogueAndLoadNextScene()
    {
        if (!isClosing)
            StartCoroutine(CloseDialogueCoroutine());
    }

    IEnumerator CloseDialogueCoroutine()
    {
        isClosing = true;
        dialogueActive = false;

        if (dialogueAnimator != null)
            dialogueAnimator.SetTrigger("Close");

        // Use unscaled time because Time.timeScale is 0 during dialogue
        yield return new WaitForSecondsRealtime(closeAnimationDuration);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        ResumeGameplay();

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