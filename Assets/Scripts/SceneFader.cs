using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string nextSceneName;

    void Start()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 1f;
            fadeOverlay.blocksRaycasts = true;
        }

        StartCoroutine(FadeIn());
    }

    public void FadeOutToNextScene()
    {
        StartCoroutine(FadeOutAndLoad(nextSceneName));
    }

    public void FadeOutToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);

        if (fadeOverlay != null)
            fadeOverlay.blocksRaycasts = false;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        yield return Fade(0f, 1f);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator Fade(float from, float to)
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.alpha = from;
        fadeOverlay.blocksRaycasts = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Lerp(from, to, time / fadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = to;
    }
}