using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;  
public class Portal : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
[Header("Portal Audio")]
public AudioSource audioSource;
public AudioClip portalSound;
public float delayBeforeLoad = 1f; // adjust to match sound

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered by: " + other.name + " | tag: " + other.tag);

        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            
			Debug.Log("Loading " + sceneToLoad);
			StartCoroutine(LoadSceneWithSound());
            //SceneManager.LoadScene(sceneToLoad);
        }
    }
private IEnumerator LoadSceneWithSound()
{
    if (audioSource != null && portalSound != null)
    {
        audioSource.PlayOneShot(portalSound);
        yield return new WaitForSeconds(delayBeforeLoad);
    }

    SceneManager.LoadScene(sceneToLoad);
}
}