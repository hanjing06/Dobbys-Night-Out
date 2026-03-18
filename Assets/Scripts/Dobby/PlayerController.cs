using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private bool canSprint = true;
     private float CurrentSpeed;
     private int numSpiders;
     
    public Vector3 playerMoveDirection;
    // Update is called once per frame
    private bool FacingRight = true;

    void Update()
    {
        move();
        
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(playerMoveDirection.x * CurrentSpeed, playerMoveDirection.y * CurrentSpeed);
        FlipController();
    }

    private void FlipController()
    {
        if (rb.linearVelocity.x < 0f && FacingRight)
        {
            Flip();
        }
        else if (rb.linearVelocity.x > 0f && !FacingRight)
        {
            Flip();
        }
    }
    private void move() {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        animator.SetFloat("moveX", inputX);
        animator.SetFloat("moveY", inputY);
        playerMoveDirection = new Vector3(inputX, inputY).normalized;
        if (playerMoveDirection == Vector3.zero)
        {
            animator.SetBool("moving", false);
        }else{
            animator.SetBool("moving", true);
        }
        if (TestIfSprinting())
        {
            animator.SetBool("running", true);
            CurrentSpeed = runSpeed;
        }else{
            animator.SetBool("running", false);
            CurrentSpeed = walkSpeed;
        }
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    
    bool TestIfSprinting()
    {
        if (!canSprint) { return false; }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return true;
        }
        return false;
    }

	public int getNumSpiders(){
		return numSpiders;

}
	public void collectSpider(){
		numSpiders++;
}
void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("Triggered with: " + other.gameObject.name);

    if (other.CompareTag("spider"))
    {
        numSpiders++;
        Debug.Log("Spider touched! Total: " + numSpiders);
        Destroy(other.gameObject);
    }
}
}
