using UnityEngine;
using UnityEngine.SceneManagement;

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager Instance;

    [Header("Noise Settings")]
    public float currentNoise = 0f;
    public float maxNoise = 100f;

    [Header("Optional Decay")]
    public bool noiseDecaysOverTime = false;
    public float decayRate = 5f;

    private void Awake()
    {
        // Simple singleton so other scripts can access this manager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (noiseDecaysOverTime && currentNoise > 0f)
        {
            currentNoise -= decayRate * Time.deltaTime;
            currentNoise = Mathf.Max(currentNoise, 0f);
        }

        if (currentNoise >= maxNoise)
        {
            RestartLevel();
        }
    }

    public void AddNoise(float amount)
    {
        currentNoise += amount;
        Debug.Log("Noise: " + currentNoise + " / " + maxNoise);
    }

    public void RestartLevel()
    {
        Invoke(nameof(ReloadScene), 1f);
    }

    private void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}