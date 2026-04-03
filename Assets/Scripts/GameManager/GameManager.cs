using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private GameObject normalNPC;
    [SerializeField] private GameObject chaseNPCPrefab;
    
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private TMP_Text timerText; 

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
