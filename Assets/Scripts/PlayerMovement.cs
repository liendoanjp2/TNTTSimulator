using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float minMoveSpeed = 2f;
    private float maxMoveSpeed = 6f;
    private float currMoveSpeed = 2f;
    private float runningMoveSpeed = 4.5f;
    private float rollingMoveSpeed = 2f; // Adds 2 movespeed to currMovespeed
    public Rigidbody2D rigidbody;
    public Animator animator;
    private Transform transform;
    private STATE playerState = STATE.NORMAL;

    private const float leftDirection = -1f;
    private const float rightDirection = 1f;

    private Vector2 movement;
    private float directionFacing = rightDirection;
    private List<string> keyPresses = new List<string>();

    private Vector2 velocity = Vector2.zero;
    private float timeSinceLastRolled = -2;
    private const int rollCooldown = 2; // 3 seconds

    // Dodge variables
    float rollDistance = 5f;

    private GameObject playerInteractableGameObject; // Gameobj Player is currently interacting with

    private HighlightController highlightController;
    private SceneController sceneController;

    public enum STATE
    {
        NORMAL, INTERACTING, STOP
    }

    public void setPlayerState(STATE state)
    {
        playerState = state;
    }

    public GameObject getPlayerInteractableGameObject()
    {
        return playerInteractableGameObject;
    }

    public void onAnimationFinish()
    {
        Interactable interactable = playerInteractableGameObject.GetComponent<Interactable>();
        interactable.onFinishInteract();
        playerInteractableGameObject = null;
        playerState = STATE.NORMAL;
        Debug.Log("Done");
    }

    private IEnumerator DodgeCoroutine()
    {
        var endOfFrame = new WaitForEndOfFrame();
        var rigidbody = GetComponent<Rigidbody2D>();
        var transform = GetComponent<Transform>();
        var boxCollider = GetComponent<BoxCollider2D>();

        Vector2 characterBoxDim = boxCollider.size;
        Vector2 distanceAbleToDodge = Vector2.one * rollDistance;

        // Moving left or right, check in front to see how far we can roll
        if(movement.x != 0)
        {
            RaycastHit2D hitX = Physics2D.Raycast(transform.position, transform.right, rollDistance);
            if (hitX && hitX.collider != null && !hitX.collider.isTrigger)
            {
                distanceAbleToDodge.x = hitX.distance - characterBoxDim.x / 2;
            }
        }
        // Moving up, check above to see how far we can roll
        if(movement.y > 0)
        {
            RaycastHit2D hitYUp = Physics2D.Raycast(transform.position, transform.up, rollDistance);
            if (hitYUp && hitYUp.collider != null && !hitYUp.collider.isTrigger)
            {
                distanceAbleToDodge.y = hitYUp.distance - characterBoxDim.y / 2;
            }
        }
        // Moving down, check below to see how far we can roll
        else if(movement.y < 0)
        {
            RaycastHit2D hitYDown = Physics2D.Raycast(transform.position, -transform.up, rollDistance);
            if (hitYDown && hitYDown.collider != null && !hitYDown.collider.isTrigger)
            {
                distanceAbleToDodge.y = hitYDown.distance - characterBoxDim.y / 2;
            }
        }

        Debug.Log(distanceAbleToDodge);

        Vector3 dodgePosition = rigidbody.position + movement.normalized * distanceAbleToDodge;

        if (movement == Vector2.zero)
        {
            dodgePosition = transform.position + transform.right.normalized * distanceAbleToDodge.x;
        }

        print(distanceAbleToDodge + "" + dodgePosition);

        animator.Play("Player_Roll");
        iTween.MoveTo(gameObject, iTween.Hash("position", dodgePosition, "time", 0.2f, "easeType", "easeInOutExpo", "oncomplete", "onDodgeComplete"));

        // Increase character movespeed
        currMoveSpeed = Mathf.Clamp(currMoveSpeed + rollingMoveSpeed, minMoveSpeed, maxMoveSpeed);
        yield return endOfFrame;

    }

    void Start()
    {
        transform = gameObject.GetComponent<Transform>();
        if (transform == null)
            throw new System.Exception("transform null");
        highlightController = gameObject.GetComponent<HighlightController>();
        if (highlightController == null)
            throw new System.Exception("highlightController null");
        sceneController = gameObject.GetComponent<SceneController>();
        if (sceneController == null)
            throw new System.Exception("sceneController null");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == STATE.NORMAL)
        {
            ProcessInputs();
            ProcessAnimations();
        }
        else if(playerState == STATE.INTERACTING || playerState == STATE.STOP)
        {
            movement = Vector2.zero;
            currMoveSpeed = minMoveSpeed;
            ProcessAnimations();
        }
    }

    void FixedUpdate()
    {
        Move();
        RaycastHit2D hitYUp = Physics2D.Raycast(transform.position, transform.up, rollDistance);
        if (hitYUp)
        {
            Debug.DrawRay(rigidbody.position, Vector2.up * hitYUp.distance, Color.red);
        }
        
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized; // Fix diagonal speed

        if(moveX != 0 || moveY != 0)
        {
            currMoveSpeed = Mathf.Clamp(currMoveSpeed + Time.deltaTime * 2, minMoveSpeed, maxMoveSpeed);
        }
        else
        {
            currMoveSpeed = Mathf.Clamp(currMoveSpeed - Time.deltaTime * 8, minMoveSpeed, maxMoveSpeed);
        }

        // deaccelerate until its zero
        velocity.x *= 0.93f;
        velocity.y *= 0.93f;


        if (Input.GetKeyDown("q"))
        {
            // Roll
            print("Q down");
            keyPresses.Add("q");
        }
        else if (Input.GetKeyDown("w"))
        {
            // Jump
            print("W down");
            keyPresses.Add("w");
        }
        else if (Input.GetKeyDown("e"))
        {
            // interact
            print("e down");
            keyPresses.Add("e");
        }

    }
  
    void ProcessAnimations()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        animator.SetFloat("Direction", directionFacing);
        animator.SetBool("isRunning", currMoveSpeed > runningMoveSpeed);
    }

    void Move()
    {
        if (directionFacing == rightDirection && movement.x < 0)
        {
            // Turn left
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
            directionFacing = leftDirection;
        }
        else if (directionFacing == leftDirection && movement.x > 0)
        {
            // Turn right
            transform.rotation = transform.rotation * Quaternion.Euler(0, -180f, 0);
            directionFacing = rightDirection;
        }

        Vector2 baseMovement = (rigidbody.position + movement * currMoveSpeed * Time.fixedDeltaTime);

        // Movement
        rigidbody.MovePosition(baseMovement);

        if (keyPresses.Count > 0)
        {
            string keyPress = keyPresses[0];
            keyPresses.RemoveAt(0);
            if (keyPress.Equals("q") && Time.fixedTime - timeSinceLastRolled > rollCooldown)
            {
                print("ROLLED");
                StartCoroutine(DodgeCoroutine());
                timeSinceLastRolled = Time.fixedTime;
            }
            else if (Input.GetKeyDown("w"))
            {
                // Jump
                print("W down");
            }
            else if (keyPress.Equals("e"))
            {
                // interact
                print("e down");
                Interact();
            }
        }
    }
    private void Interact() { 
        Collider2D farthestCollider = highlightController.getFarthestCollider();
        if (farthestCollider)
        {
            Interactable hit = farthestCollider.GetComponent<Interactable>();
            if (hit != null)
            {
                playerInteractableGameObject = farthestCollider.gameObject;
                hit.Interact(this);
                return;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("oncollisionenter2d: " + collision.gameObject.name);
        Minigame minigame = collision.gameObject.GetComponent<Minigame>();
        if (minigame != null) {
            minigame.load(sceneController);
        }
    }
}


