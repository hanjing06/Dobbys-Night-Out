using UnityEngine;
using UnityEngine.UI;
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

    [Header("Noise Bar Sprites")]
    public Sprite noise0;
    public Sprite noise20;
    public Sprite noise40;
    public Sprite noise60;
    public Sprite noise80;
    public Sprite noise100;

    public Image noiseBar;

    private bool restarting = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateNoiseBar();
    }

    private void Update()
    {
        if (noiseDecaysOverTime && currentNoise > 0f)
        {
            currentNoise -= decayRate * Time.deltaTime;
            currentNoise = Mathf.Max(currentNoise, 0f);
            UpdateNoiseBar();
        }

        if (currentNoise >= maxNoise && !restarting)
        {
            currentNoise = maxNoise;
            UpdateNoiseBar();
            RestartLevel();
        }
    }

    public void AddNoise(float amount)
    {
        currentNoise += amount;
        currentNoise = Mathf.Clamp(currentNoise, 0f, maxNoise);

        Debug.Log("Noise: " + currentNoise + " / " + maxNoise);
        UpdateNoiseBar();
    }

    public void ReduceNoise(float amount)
    {
        currentNoise -= amount;
        currentNoise = Mathf.Clamp(currentNoise, 0f, maxNoise);
        UpdateNoiseBar();
    }

    private void UpdateNoiseBar()
    {
        if (noiseBar == null)
            return;

        float percent = (currentNoise / maxNoise) * 100f;

        if (percent <= 0f)
        {
            noiseBar.sprite = noise0;
        }
        else if (percent <= 20f)
        {
            noiseBar.sprite = noise20;
        }
        else if (percent <= 40f)
        {
            noiseBar.sprite = noise40;
        }
        else if (percent <= 60f)
        {
            noiseBar.sprite = noise60;
        }
        else if (percent <= 80f)
        {
            noiseBar.sprite = noise80;
        }
        else
        {
            noiseBar.sprite = noise100;
        }
    }

    public void RestartLevel()
    {
        restarting = true;
        Invoke(nameof(ReloadScene), 1f);
    }

    private void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}