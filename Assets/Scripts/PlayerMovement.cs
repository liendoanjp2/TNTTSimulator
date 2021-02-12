using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Constants
    private const float leftDirection = -1f;
    private const float rightDirection = 1f;

    // movement
    private float minMoveSpeed = 3f;                 // Minimum movespeed
    private float maxMoveSpeed = 6f;                 // Maximum movespeed
    public float runningMoveSpeed = 5.5f;            // Speed at which to display running animation
    public float currMoveSpeed = 2f;                 // Current movement
    private float msAccelerationMin = 4f;            // Min acceleration for movement
    private float msAccelerationCurr = 15f;           // Current acceleration for movement
    private float msAccelerationMax = 5f;           // Max acceleration for movement
    private float msAccelerationStartTime = 0;       // Start time of first acceleration
    private float msAccelerationTime = 1.2f;         // Time to accelerate to msAccelerationMax
    private float movespeedDeacceleration = 25f;     // deacceleration rate
    private float timeSinceLastMovement;             // Time since the last movement
    private float timeToDeaccelerateSpeed = 0.05f;    // Amount of time in seconds to wait until we deaccelerate the player's speed
    public Vector2 movement;

    // Direction
    public float directionFacing = rightDirection;
    
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

        // Handle direction of player
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

        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider2D.size, 0, movement, rollDistance);
        if (hit)
        {
            Debug.DrawRay(rigidbody.position, movement * hit.distance, Color.red);
        }
        
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 newMovement = new Vector2(moveX, moveY).normalized; // Fix diagonal speed

        if(newMovement == Vector2.zero)
        {
            // If we have let go of the controls, we should deaccelerate in that direction
            // Slowly deaccelerate speed if we havent moved in awhile
            if (Time.time - timeSinceLastMovement >= timeToDeaccelerateSpeed)
            {
                currMoveSpeed = Mathf.Clamp(currMoveSpeed - Time.deltaTime * movespeedDeacceleration, minMoveSpeed, maxMoveSpeed);
            }

            // If our speed is zero then set our movement to have no movement
            if(currMoveSpeed == minMoveSpeed)
            {
                movement = newMovement;
            }
        }
        else
        {
            if(movement == Vector2.zero)
            {
                // We began to move
                msAccelerationStartTime = Time.time;
            }

            // Update to the new movement
            movement = newMovement;

            // Determine the acceleration step
            float accelerationTime = (Time.time - msAccelerationStartTime) / msAccelerationTime;
            msAccelerationCurr = Mathf.SmoothStep(msAccelerationMin, msAccelerationMax, accelerationTime);

            // Update the current move speed
            currMoveSpeed = Mathf.Clamp(currMoveSpeed + Time.deltaTime * msAccelerationCurr, minMoveSpeed, maxMoveSpeed);
            
            // Update the timeSinceLastMovement because we have moved
            timeSinceLastMovement = Time.time;
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
        moveToRoutine = StartCoroutine(MoveToCoroutine(coords, time));
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

        // Remove the sign because we want distance to be nonscalar, these will help us know the distance to dodge.
        Vector2 absRightDirection = new Vector2(Mathf.Abs(transform.right.x), transform.right.y);

        // If we are moving we will cast raycast in direction of movement
        if (movement != Vector2.zero)
        {
            // Raycast in the direction of movement and see if theres any object infront, then limit our roll
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, characterBoxDim, 0, movement, rollDistance);
            if (hit)
            {
                // Calculate the distance (if we were to go diagonally), remove the x and y component if we arent moving diagonally.
                distanceAbleToDodge = Vector2.one * hit.distance;

                if (movement.x == 0)
                {
                    // Not moving in x direction, so we assume we are not to dodge in the x direction
                    distanceAbleToDodge.x = 0;
                }
                if(movement.y == 0)
                {
                    // Not moving in y direction, so we assume we are not to dodge in the x direction
                    distanceAbleToDodge.y = 0;
                }
            }
        }
        else
        {
            // Otherwise we cast raycast in direction we are facing
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, characterBoxDim, 0, transform.right, rollDistance);
            if (hit)
            {
                // Remove the sign because we want distance to be nonscalar
                distanceAbleToDodge = absRightDirection * hit.distance;
            }
        }


        Debug.Log(distanceAbleToDodge);

        Vector3 dodgePosition = rigidbody.position + movement * distanceAbleToDodge;

        if (movement == Vector2.zero)
        {
            dodgePosition = transform.position + transform.right.normalized * distanceAbleToDodge.x;
        }

        print(distanceAbleToDodge + "|" + dodgePosition);

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


