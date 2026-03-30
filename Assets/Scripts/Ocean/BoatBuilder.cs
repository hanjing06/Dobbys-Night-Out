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
    public GameObject hullStage1;
    public GameObject hullStage2;
    public GameObject leftPaddle;
    public GameObject rightPaddle;

    private int visualProgress = 0;

    public void AddProgress(int amount)
    {
        visualProgress += amount;
        RefreshBoat();
    }

    void RefreshBoat()
    {
        if (hullStage1 != null) hullStage1.SetActive(visualProgress >= 3);
        if (hullStage2 != null) hullStage2.SetActive(visualProgress >= 6);
        if (leftPaddle != null) leftPaddle.SetActive(visualProgress >= 9);
        if (rightPaddle != null) rightPaddle.SetActive(visualProgress >= 12);
    }
}