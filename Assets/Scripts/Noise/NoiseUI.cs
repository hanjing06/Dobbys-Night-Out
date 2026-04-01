using UnityEngine;
using TMPro;

public class NoiseUI : MonoBehaviour
{
    public TextMeshProUGUI noiseText;

    private void Update()
    {
        if (NoiseManager.Instance != null && noiseText != null)
        {
            noiseText.text = "Noise: " + Mathf.RoundToInt(NoiseManager.Instance.currentNoise) 
                                       + " / " + Mathf.RoundToInt(NoiseManager.Instance.maxNoise);
        }
    }
}