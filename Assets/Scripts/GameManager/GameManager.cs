using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private GameObject normalNPC;
    [SerializeField] private GameObject chaseNPCPrefab;
    

    private bool hasSwapped = false;

    void Awake()
    {
        Instance = this;
    }

    public void OnPlayerLeftRoom()
    {
        if (hasSwapped) return;

        hasSwapped = true;

        SwapNPC();
    }

    void SwapNPC()
    {
        if (normalNPC != null)
        {
            Destroy(normalNPC);
        }

        if (normalNPC == null || chaseNPCPrefab == null)
            return;

        // Save old position + rotation
        Vector3 spawnPos = normalNPC.transform.position;
        Quaternion spawnRot = normalNPC.transform.rotation;

        // Destroy old NPC
        Destroy(normalNPC);

        // Spawn chase version in same place
        Instantiate(chaseNPCPrefab, spawnPos, spawnRot);
    }
}
