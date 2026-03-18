using UnityEngine;

public class SpiderSpawner : MonoBehaviour
{
    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private int numberToSpawn = 10;

    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;

    void Start()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector2 spawnPosition = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            Instantiate(spiderPrefab, spawnPosition, Quaternion.identity);
        }
    }
}