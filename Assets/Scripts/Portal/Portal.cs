using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered by: " + other.name + " | tag: " + other.tag);

        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Loading " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}