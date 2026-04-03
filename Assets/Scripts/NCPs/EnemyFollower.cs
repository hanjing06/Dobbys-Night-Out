using UnityEngine;
public class EnemyFollower : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float reachDistance = 0.3f;
    [SerializeField] private Animator animator;

    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float speedIncreaseRate = 0.05f;
    [SerializeField] private float speedGrowthDelay = 3f;
    private Vector3 lockedDirection;
    private float speedTimer = 0f;

    private Vector3 currentTarget;
    private bool hasTarget = false;

    void Update() // 👈 switched from FixedUpdate
    {
        IncreaseSpeedOverTime();
        FollowPlayerPath();
        HandleAnimation();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.ResetGame();
        }
    }
    void FollowPlayerPath()
    {
        if (PlayerTracker.PositionQueue.Count == 0)
        {
            hasTarget = false;
            return;
        }

        if (!hasTarget)
        {
            currentTarget = PlayerTracker.PositionQueue.Peek();
            hasTarget = true;

            Vector3 diff = currentTarget - transform.position;

            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                lockedDirection = new Vector3(Mathf.Sign(diff.x), 0, 0);
            else
                lockedDirection = new Vector3(0, Mathf.Sign(diff.y), 0);
        }

        Vector3 toTarget = currentTarget - transform.position;

        // 🔥 If we passed the target → reset
        if (Vector3.Dot(toTarget, lockedDirection) < 0)
        {
            hasTarget = false;
            return;
        }

        // Move
        transform.position += lockedDirection * moveSpeed * Time.deltaTime;

        // 🔥 Snap when close
        if (Vector3.Distance(transform.position, currentTarget) <= reachDistance)
        {
            transform.position = currentTarget;

            PlayerTracker.PositionQueue.Dequeue();
            hasTarget = false;
        }
    }

    void IncreaseSpeedOverTime()
    {
        if (PlayerTracker.PositionQueue.Count == 0)
            return;

        speedTimer += Time.deltaTime;

        if (speedTimer < speedGrowthDelay)
            return;

        moveSpeed += speedIncreaseRate * Time.deltaTime;
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

