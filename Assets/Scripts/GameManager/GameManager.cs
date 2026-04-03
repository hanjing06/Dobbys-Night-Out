using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private GameObject normalNPC;
    [SerializeField] private GameObject chaseNPCPrefab;
    
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private TMP_Text timerText; 

    private bool hasSwapped = false;
    private float timeRemaining = 300f; // 5 minutes
    private bool timerRunning = false;
    void Start()
    {
        timerPanel.SetActive(false);
    }
    void Awake()
    {
        Instance = this;
    }

    public void OnPlayerLeftRoom()
    {
        if (hasSwapped) return;

        hasSwapped = true;
        SwapNPC();
        
        timerPanel.SetActive(true);
        timerRunning = true;
    }

    void SwapNPC()
    {
        if (normalNPC != null)
        {
            Destroy(normalNPC);
        }

        if (normalNPC == null || chaseNPCPrefab == null)
            return;

        // Save old position + rotation
        Vector3 spawnPos = normalNPC.transform.position;
        Quaternion spawnRot = normalNPC.transform.rotation;

        // Destroy old NPC
        Destroy(normalNPC);

        // Spawn chase version in same place
        Instantiate(chaseNPCPrefab, spawnPos, spawnRot);
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
            
        }
        void UpdateTimerDisplay(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
