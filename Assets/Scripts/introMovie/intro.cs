using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class intro : MonoBehaviour
{
    public Transform player;
    public Transform cat;
    public Transform stopPoint;
    public Transform exitPoint;

    public GameObject[] thoughtBubbles;

    public float walkSpeed = 2f;
    public string nextSceneName = "Level1";

    private bool walkingIn = true;
    private bool showingThoughts = false;
    private bool walkingAway = false;

    private int currentBubbleIndex = 0;

    void Start()
    {
        foreach (GameObject bubble in thoughtBubbles)
        {
            if (bubble != null)
            {
                bubble.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (walkingIn)
        {
            WalkPlayerIn();
        }
        else if (showingThoughts)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceBubble();
            }
        }
        else if (walkingAway)
        {
            WalkAwayTogether();
            //yield return new WaitForSeconds(3f);
            
            //SceneManager.LoadScene(nextSceneName);
        }
    }

    void WalkPlayerIn()
    {
        player.position = Vector3.MoveTowards(
            player.position,
            stopPoint.position,
            walkSpeed * Time.deltaTime
        );

        if (Vector3.Distance(player.position, stopPoint.position) < 0.05f)
        {
            player.position = stopPoint.position;
            walkingIn = false;
            StartCoroutine(StartThoughtSequence());
        }
    }

    IEnumerator StartThoughtSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (thoughtBubbles.Length > 0 && thoughtBubbles[0] != null)
        {
            thoughtBubbles[0].SetActive(true);
            showingThoughts = true;
        }
    }

    void AdvanceBubble()
    {
        if (currentBubbleIndex < thoughtBubbles.Length && thoughtBubbles[currentBubbleIndex] != null)
        {
            thoughtBubbles[currentBubbleIndex].SetActive(false);
        }

        currentBubbleIndex++;

        if (currentBubbleIndex < thoughtBubbles.Length)
        {
            if (thoughtBubbles[currentBubbleIndex] != null)
            {
                thoughtBubbles[currentBubbleIndex].SetActive(true);
            }
        }
        else
        {
            showingThoughts = false;
            walkingAway = true;
        }
    }

    void WalkAwayTogether()
    {
        float playerNewX = Mathf.MoveTowards(
            player.position.x,
            exitPoint.position.x,
            walkSpeed * Time.deltaTime
        );

        player.position = new Vector3(
            playerNewX,
            player.position.y,
            player.position.z
        );

        float catTargetX = player.position.x - 1f;

        float catNewX = Mathf.MoveTowards(
            cat.position.x,
            catTargetX,
            walkSpeed * Time.deltaTime
        );

        cat.position = new Vector3(
            catNewX,
            cat.position.y,
            cat.position.z
        );

        Debug.Log("Player X: " + player.position.x + " Exit X: " + exitPoint.position.x);

        if (Mathf.Abs(player.position.x - exitPoint.position.x) <= 0.05f)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}