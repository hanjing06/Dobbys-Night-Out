using UnityEngine;

public class RoomExitTrigger : MonoBehaviour
{
    public static bool playerInsideRoom = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideRoom = false;

            // Notify manager when player leaves
            GameManager.Instance?.OnPlayerLeftRoom();
        }
    }
}
