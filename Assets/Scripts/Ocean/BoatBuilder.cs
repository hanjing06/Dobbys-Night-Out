using UnityEngine;
using System.Collections;

/*
Stage 0
Only water, no boat
   
Stage 1
Small partial wooden hull
   
Stage 2
Full wooden hull
   
Stage 3
Left paddle added
   
Stage 4
Right paddle added
   
Stage 5
Finished boat with a little bounce / shine
 */

public class BoatBuilder : MonoBehaviour
{
    [Header("Boat Parts")]
    public GameObject hullStage1;
    public GameObject hullStage2;
    public GameObject leftPaddle;
    public GameObject rightPaddle;

    private int progress = 0;

    public void AddProgress(int amount)
    {
        progress += amount;
        if (progress > 12) progress = 12;

        Debug.Log("Boat Progress: " + progress);

        UpdateBoat();
    }

    void UpdateBoat()
    {
        if (progress >= 3)
            ActivatePart(hullStage1);

        if (progress >= 6)
            ActivatePart(hullStage2);

        if (progress >= 9)
            ActivatePart(leftPaddle);

        if (progress >= 12)
            ActivatePart(rightPaddle);
    }

    void ActivatePart(GameObject part)
    {
        if (part == null) return;

        if (!part.activeSelf)
        {
            StartCoroutine(BuildAnimation(part));
        }
    }

    IEnumerator BuildAnimation(GameObject part)
    {
        part.SetActive(true);

        Vector3 originalScale = part.transform.localScale;
        part.transform.localScale = Vector3.zero;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            part.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale * 1.1f, t);
            yield return null;
        }

        time = 0f;
        Vector3 overshoot = originalScale * 1.1f;

        while (time < 0.1f)
        {
            time += Time.deltaTime;
            float t = time / 0.1f;

            part.transform.localScale = Vector3.Lerp(overshoot, originalScale, t);
            yield return null;
        }

        part.transform.localScale = originalScale;
    }
}