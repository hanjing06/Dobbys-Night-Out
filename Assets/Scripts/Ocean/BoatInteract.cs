using System.Collections;
using UnityEngine;

public class BoatInteract : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Game gameManager;
    [SerializeField] private Transform boatRoot;
    [SerializeField] private Transform player;

    [Header("Sailing")]
    [SerializeField] private float sailSpeed = 2f;
    [SerializeField] private float sailDuration = 2.5f;
    [SerializeField] private Vector3 sailDirection = new Vector3(1f, 0f, 0f);

    private bool hasSailed = false;

    public bool CanInteract()
    {
        return !hasSailed;
    }

    public void Interact()
    {
        if (hasSailed) return;

        Debug.Log("Boat interaction triggered!");
        hasSailed = true;

        StartCoroutine(SailRoutine());
    }

    IEnumerator SailRoutine()
    {
        float timer = 0f;
        Vector3 dir = sailDirection.normalized;

        while (timer < sailDuration)
        {
            timer += Time.deltaTime;

            if (boatRoot != null)
                boatRoot.position += dir * sailSpeed * Time.deltaTime;

            if (player != null)
                player.position += dir * sailSpeed * Time.deltaTime;

            yield return null;
        }

        if (gameManager != null)
            gameManager.SailToNextLevel();
    }
}