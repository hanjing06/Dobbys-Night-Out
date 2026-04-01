using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public static Queue<Vector3> PositionQueue = new Queue<Vector3>();
    [SerializeField] private float recordInterval = 0.15f;
    [SerializeField] private float minDistance = 0.3f;
    [SerializeField] private int maxQueueSize = 100;

    private float timer = 0f;
    private Vector3 lastRecordedPosition;

    void Start()
    {
        lastRecordedPosition = transform.position;
        PositionQueue.Enqueue(transform.position);
    }

    void Update()
    {
        if (RoomExitTrigger.playerInsideRoom) {return;}
        timer += Time.deltaTime;

        if (timer >= recordInterval)
        {
            float distance = Vector3.Distance(transform.position, lastRecordedPosition);

            // Only record if player actually moved enough
            if (distance >= minDistance)
            {
                PositionQueue.Enqueue(transform.position);
                lastRecordedPosition = transform.position;

                // Prevent infinite growth
                if (PositionQueue.Count > maxQueueSize)
                {
                    PositionQueue.Dequeue();
                }
            }

            timer = 0f;
        }
    }
}
