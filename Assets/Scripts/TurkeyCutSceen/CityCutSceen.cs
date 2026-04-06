using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class CityCutSceen : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(NextLevel()); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(22.32f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
