using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Constants
    private float minMoveSpeed = 2f;
    private float maxMoveSpeed = 6f;
    public float runningMoveSpeed = 4.5f;

    private const float leftDirection = -1f;
    private const float rightDirection = 1f;

    // Direction + movespeed + movement
    public float currMoveSpeed = 2f;
    public float directionFacing = rightDirection;
    public Vector2 movement;

    // Keypresses for input
    private List<string> keyPresses = new List<string>();

    // Dodge variables
    private float rollingMoveSpeed = 2f; // Adds 2 movespeed to currMovespeed
    private int rollCooldown = 2; // 3 seconds
    private float rollDistance = 5f;
    private float timeSinceLastRolled = -2;

    // Components from player
    new private Rigidbody2D rigidbody;
    private AnimationController animationController;
    new private Transform transform;
    private HighlightController highlightController;
    private SceneController sceneController;
    private PlayerState playerState;

    private Coroutine moveToRoutine;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if (rigidbody == null)
            throw new System.Exception("rigidbody null");
        transform = gameObject.GetComponent<Transform>();
        if (transform == null)
            throw new System.Exception("transform null");
        highlightController = gameObject.GetComponent<HighlightController>();
        if (highlightController == null)
            throw new System.Exception("highlightController null");
        sceneController = gameObject.GetComponent<SceneController>();
        if (sceneController == null)
            throw new System.Exception("sceneController null");
        animationController = gameObject.GetComponent<AnimationController>();
        if (animationController == null)
            throw new System.Exception("animationController null");
        playerState = gameObject.GetComponent<PlayerState>();
        if (playerState == null)
            throw new System.Exception("playerState null");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerState.STATE state = playerState.getPlayerState();
        if (state == PlayerState.STATE.NORMAL)
        {
            ProcessInputs();
        }
        else if(state == PlayerState.STATE.INTERACTING || state == PlayerState.STATE.STOP)
        {
            movement = Vector2.zero;
            currMoveSpeed = minMoveSpeed;
        }
        else if(state == PlayerState.STATE.CHURCHMINIGAME)
        {
            
        }
    }

    void FixedUpdate()
    {
        if (moveToRoutine == null)
        {
            Move();
        }
        
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
            else if (keyPress.Equals("w"))
            {
                // Jump
                print("W key process");
                MoveTo(new Vector2(0, 0), 2f);
            }
            else if (keyPress.Equals("e"))
            {
                // interact
                print("e down");
                highlightController.Interact();
            }
        }
    }

    public void MoveTo(Vector3 coords, float time)
    {
        if(moveToRoutine == null)
        {
            moveToRoutine = StartCoroutine(MoveToCoroutine(coords, time));
        }
        else
        {
            Debug.Log("moveToRoutine still running!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("oncollisionenter2d: " + collision.gameObject.name);
        SceneTransport transport = collision.gameObject.GetComponent<SceneTransport>();
        if (transport != null) {
            transport.load(sceneController);
        }
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
        if (movement.x != 0)
        {
            RaycastHit2D hitX = Physics2D.Raycast(transform.position, transform.right, rollDistance);
            if (hitX && hitX.collider != null && !hitX.collider.isTrigger)
            {
                distanceAbleToDodge.x = hitX.distance - characterBoxDim.x / 2;
            }
        }
        // Moving up, check above to see how far we can roll
        if (movement.y > 0)
        {
            RaycastHit2D hitYUp = Physics2D.Raycast(transform.position, transform.up, rollDistance);
            if (hitYUp && hitYUp.collider != null && !hitYUp.collider.isTrigger)
            {
                distanceAbleToDodge.y = hitYUp.distance - characterBoxDim.y / 2;
            }
        }
        // Moving down, check below to see how far we can roll
        else if (movement.y < 0)
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

        animationController.playRoll();
        iTween.MoveTo(gameObject, iTween.Hash("position", dodgePosition, "time", 0.2f, "easeType", "easeInOutExpo", "oncomplete", "onDodgeComplete"));

        // Increase character movespeed
        currMoveSpeed = Mathf.Clamp(currMoveSpeed + rollingMoveSpeed, minMoveSpeed, maxMoveSpeed);
        yield return endOfFrame;

    }

    private IEnumerator MoveToCoroutine(Vector3 coords, float time)
    {
        var endOfFrame = new WaitForEndOfFrame();
        Debug.Log(coords);
        Vector3 playerPosition = gameObject.transform.position;
        if(coords.x < playerPosition.x)
        {
            movement = new Vector2(-1, 0);
        }
        else
        {
            movement = new Vector2(1, 0);
        }
        
        iTween.MoveTo(gameObject, iTween.Hash("position", coords, "time", time, "oncomplete", "onMoveToComplete", "easeType", "easeInCubic"));
        yield return endOfFrame;
    }

    private void onMoveToComplete()
    {
        movement = Vector2.zero;
        moveToRoutine = null;
        Debug.Log("onMoveToComplete");
    }

    public bool isMoveToOn()
    {
        return moveToRoutine != null ? true : false;
    }
}


