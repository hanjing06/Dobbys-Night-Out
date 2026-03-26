using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector: MonoBehaviour
{
    private IInteractable interactableInRange =null;
    public GameObject interactionIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Interact button pressed");
            interactableInRange?.Interact();
        }
    }
    public void Interact()
    {
        Debug.Log("Trying to interact...");
        interactableInRange?.Interact();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered trigger with: " + collision.name);

        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
            Debug.Log("Interactable detected!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
