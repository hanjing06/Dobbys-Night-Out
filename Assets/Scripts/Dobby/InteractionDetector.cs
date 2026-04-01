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

        if (interactableInRange == null)
        {
            Debug.LogWarning("No interactable in range.");
            return;
        }

        Debug.Log("Calling Interact() on object in range.");
        interactableInRange.Interact();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered trigger with: " + collision.name);

        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable == null)
            interactable = collision.GetComponentInParent<IInteractable>();

        if (interactable != null && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
            Debug.Log("Interactable detected!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable == null)
            interactable = collision.GetComponentInParent<IInteractable>();

        if (interactable != null && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
