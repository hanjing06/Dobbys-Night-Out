using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private InteractionDetector interactionDetector;
    [SerializeField] private CurrencyManager currencyManager;
    
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private bool canSprint = true;
     private float CurrentSpeed;
     
    public Vector3 playerMoveDirection;
    // Update is called once per frame
    private bool FacingRight = true;
    
    //HOGWARTS LEVEL: INTERACTING WITH THE SCROLL
    public GameObject box;
    public TMP_Text displayMessage;
    private PuzzleManager puzzleManager;
    public GameObject puzzle;
    public bool unlockScroll;
    void Start()
    {
        currencyManager = GameObject.Find("CurrencyCanvas").GetComponent<CurrencyManager>();
        puzzleManager = puzzle.GetComponent<PuzzleManager>();
        
    }
    void Update()
    {
        move();
        Interaction();
        
        //hide display message
        if (puzzleManager.isActive)
        {
            HideMessage();
        }
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
            Debug.Log("not moving");
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

    private void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactionDetector?.Interact();
        } else if (Input.GetKeyDown(KeyCode.C) && unlockScroll)
        {
            puzzleManager.isActive = true;
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
   
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered with: " + other.gameObject.name);

        if (other.CompareTag("spider"))
        {
            Debug.Log("Spider touched! Total: " + currencyManager.numSpiders);
            Destroy(other.gameObject);
            currencyManager.CollectCurrency(1);
        } else if (other.CompareTag("scroll"))
        {
            Debug.Log("Touched scroll!");
            box.GetComponent<Image>().enabled = true;
            displayMessage.GetComponent<TextMeshProUGUI>().enabled = true;
            displayMessage.text = "Press C to interact";
            unlockScroll = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("scroll"))
        {
            Debug.Log("Leaving scroll!");
            unlockScroll = false;
            HideMessage();
        }
    }

    void HideMessage()
    {
        box.GetComponent<Image>().enabled = false;
        displayMessage.GetComponent<TextMeshProUGUI>().enabled = false;
    }
}
