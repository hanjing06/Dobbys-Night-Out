using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject player;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool PlayerIsAtTheDoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerIsAtTheDoor = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerIsAtTheDoor = false;
        }
    }
    public void Interact()
    {
        Debug.Log("Door interacted!");
        SceneManager.LoadScene(sceneToLoad);
    }
    public bool CanInteract()
    {
        return true;
    }
}
