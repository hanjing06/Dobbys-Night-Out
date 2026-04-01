using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float reachDistance = 0.3f;
    [SerializeField] private Animator animator;
    
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float speedIncreaseRate = 0.05f; // VERY slow
    [SerializeField] private float speedGrowthDelay = 3f; // wait before ramping

    private float speedTimer = 0f;

    private Vector3 currentTarget;
    private bool hasTarget = false;

    private Vector3 lastPosition;

    void Update()
    {
        IncreaseSpeedOverTime();
        FollowPlayerPath();
        HandleAnimation();
    }

    void FollowPlayerPath()
    {
        if (PlayerTracker.PositionQueue.Count == 0)
            return;

        if (!hasTarget)
        {
            currentTarget = PlayerTracker.PositionQueue.Peek();
            hasTarget = true;
        }

        // Move toward target
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget,
            moveSpeed * Time.deltaTime
        );

        // Check if reached
        if (Vector3.Distance(transform.position, currentTarget) <= reachDistance)
        {
            PlayerTracker.PositionQueue.Dequeue();
            hasTarget = false;
        }
    }
    void IncreaseSpeedOverTime()
    {
        if (PlayerTracker.PositionQueue.Count == 0)
            return; // only grow while chasing

        speedTimer += Time.deltaTime;

        // wait a bit before starting speed increase
        if (speedTimer < speedGrowthDelay)
            return;

        moveSpeed += speedIncreaseRate * Time.deltaTime;

        // clamp so it never becomes unfair
        moveSpeed = Mathf.Min(moveSpeed, maxMoveSpeed);
    }
    void HandleAnimation()
    {
        if (animator == null) return;

        if (!hasTarget)
        {
            animator.SetBool("moving", false);
            return;
        }

        // Direction toward target (much more stable than delta movement)
        Vector3 direction = (currentTarget - transform.position).normalized;

        bool isMoving = Vector3.Distance(transform.position, currentTarget) > reachDistance;

        animator.SetBool("moving", isMoving);

        if (isMoving)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", 0);
            }
            else
            {
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", direction.y);
            }
        }
    }
}

