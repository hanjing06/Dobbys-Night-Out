using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private GameObject normalNPC;
    [SerializeField] private GameObject chaseNPCPrefab;
    
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private TMP_Text timerText; 

    [SerializeField] private GameObject InfoPanel;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float displayTime = 2f;
    
    private UnityEngine.UI.Image infoPanelImage;
    
    private bool hasSwapped = false;
    private float timeRemaining = 120f; // 2 minutes
    private bool timerRunning = false;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timerPanel.SetActive(false);

        timerPanel.SetActive(false);

        InfoPanel.SetActive(true);
        
        infoPanelImage = InfoPanel.GetComponent<UnityEngine.UI.Image>();

        StartCoroutine(FadeOutInfoPanel());
    }
    void SwapNPC()
    {
        if (normalNPC == null || chaseNPCPrefab == null)
            return;

        Vector3 spawnPos = normalNPC.transform.position;
        Quaternion spawnRot = normalNPC.transform.rotation;

        Destroy(normalNPC);

        Instantiate(chaseNPCPrefab, spawnPos, spawnRot);
    }


    public void OnPlayerLeftRoom()
    {
        if (hasSwapped) return;

        hasSwapped = true;
        SwapNPC();
        
        timerPanel.SetActive(true);
        timerRunning = true;
    }

    void Update()
    {
        if (!timerRunning) return;

        if (timeRemaining > 0)
        {
            if (timeRemaining <= 60f)
            {
                timerText.color = Color.red;
            }

            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            timerRunning = false;

            UpdateTimerDisplay(0);
            Debug.Log("Time's up!");
            ResetGame();
        }
    }
    IEnumerator FadeOutInfoPanel()
    {
        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        Color panelColor = infoPanelImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeDuration);

            panelColor.a = alpha;
            infoPanelImage.color = panelColor;

            yield return null;
        }

        InfoPanel.SetActive(false);
    }
    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ResetGame()
    {
        PlayerTracker.ResetTracker();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
