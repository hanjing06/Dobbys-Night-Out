using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private float startDialogueTime = 1f;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Animator dialogueAnimator;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text dialogueText;


    [Header("Dialogue Lines")]
    [SerializeField] private DialogueLine[] lines;

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName;

    [Header("Pause Gameplay")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private bool pauseGameWithTimeScale = true;

    [Header("Auto Find Player")]
    [SerializeField] private string playerTag = "Player";

    [Header("Animation Settings")]
    [SerializeField] private float closeAnimationDuration = 0.5f;

    private int currentLineIndex;
    private bool dialogueStarted;
    private bool dialogueActive;
    private bool isClosing;
    private int startFrame = -1;

    private float savedAnimatorSpeed = 1f;

    private static readonly int OpenHash = Animator.StringToHash("Open");
    private static readonly int CloseHash = Animator.StringToHash("Close");

    private void Start()
    {
        FindPlayerReferences();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueStarted)
            CheckTimelineStart();

        if (!dialogueActive || isClosing)
            return;

        if (Time.frameCount == startFrame)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            NextDialogue();
    }

    private void CheckTimelineStart()
    {
        if (timelineDirector == null)
            return;

        if (timelineDirector.state == PlayState.Playing && timelineDirector.time >= startDialogueTime)
            StartDialogue();
    }

    public void StartDialogue()
    {
        if (dialogueStarted || lines == null || lines.Length == 0)
        {
            if (lines == null || lines.Length == 0)
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
            dialogueAnimator.SetTrigger(OpenHash);
    }

    private void ShowCurrentLine()
    {
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

    private void PauseGameplay()
    {
        if (playerController != null)
            playerController.enabled = false;

        if (playerRigidbody2D != null)
        {
            playerRigidbody2D.linearVelocity = Vector2.zero;
            playerRigidbody2D.angularVelocity = 0f;
            playerRigidbody2D.simulated = false;
        }

        if (playerAnimator != null)
        {
            savedAnimatorSpeed = playerAnimator.speed;
            playerAnimator.speed = 0f;
        }

        if (pauseGameWithTimeScale)
            Time.timeScale = 0f;
    }

    private void ResumeGameplay()
    {
        if (pauseGameWithTimeScale)
            Time.timeScale = 1f;

        if (playerController != null)
            playerController.enabled = true;

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

    private IEnumerator CloseDialogueCoroutine()
    {
        isClosing = true;
        dialogueActive = false;

        if (dialogueAnimator != null)
            dialogueAnimator.SetTrigger(CloseHash);

        yield return new WaitForSecondsRealtime(closeAnimationDuration);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        ResumeGameplay();

        if (!string.IsNullOrWhiteSpace(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogWarning("Next scene name is empty. Assign it in the Inspector.");
    }

    private void FindPlayerReferences()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject == null)
        {
            Debug.LogWarning($"Player with tag '{playerTag}' was not found.");
            return;
        }

        if (playerController == null)
            playerController = playerObject.GetComponent<PlayerController>();

        if (playerRigidbody2D == null)
            playerRigidbody2D = playerObject.GetComponent<Rigidbody2D>();

        if (playerAnimator == null)
            playerAnimator = playerObject.GetComponent<Animator>();
    }
}