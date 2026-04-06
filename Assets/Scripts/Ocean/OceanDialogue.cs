using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OceanDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private GameObject di;

    [SerializeField] private GameObject[] d;
    private int current = -1;
    private bool input = false;
    public string nextScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DelayInput());
        for (int i = 0; i < d.Length; i++)
        {
            d[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (input)
        {
            if (Input.anyKeyDown)
            { 
                if (current == -1)
                {
                    if (di != null)
                        di.SetActive(false);

                    current = 0;

                    if (d.Length > 0)
                    {
                        d[current].SetActive(true);
                    }
                    else
                    {
                        LoadNextScene();
                    }

                    return;
                }

                // hide current dialogue from array
                d[current].SetActive(false);

                current++;

                // if no more dialogues, load next scene
                if (current >= d.Length)
                {
                    LoadNextScene();
                    return;
                }

                // show next one
                d[current].SetActive(true);
            }
            
        }
    }

    IEnumerator DelayInput()
    {
        yield return new WaitForSeconds(8f);
        input = true;
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
    
}
