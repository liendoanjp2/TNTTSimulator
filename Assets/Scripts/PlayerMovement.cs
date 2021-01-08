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

    private Coroutine _dodging;


    enum STATE
    {
        NORMAL, ROLLING
    }


    private IEnumerator DodgeCoroutine()
    {
        var endOfFrame = new WaitForEndOfFrame();
        var rigidbody = GetComponent<Rigidbody2D>();
        var transform = GetComponent<Transform>();
        var boxCollider = GetComponent<BoxCollider2D>();
        Vector2 characterBoxDim = boxCollider.size;

        Vector2 distanceAbleToDodge = Vector2.one * rollDistance;

        RaycastHit2D hitX = Physics2D.Raycast(transform.position, transform.right, rollDistance);
        if (hitX.collider != null)
        {
            distanceAbleToDodge.x = hitX.distance - characterBoxDim.x / 2;
        }

        RaycastHit2D hitYUp = Physics2D.Raycast(transform.position, transform.up, rollDistance);
        if (hitYUp.collider != null)
        {
            distanceAbleToDodge.y = hitYUp.distance - characterBoxDim.y / 2;
        }

        RaycastHit2D hitYDown = Physics2D.Raycast(transform.position, -transform.up, rollDistance);
        if (hitYDown.collider != null)
        {
            distanceAbleToDodge.y = hitYDown.distance - characterBoxDim.y / 2;
        }

        Vector3 dodgePosition = rigidbody.position + movement.normalized * distanceAbleToDodge;

        if (movement == Vector2.zero)
        {
            dodgePosition = transform.position + transform.right.normalized * distanceAbleToDodge.x;
        }

        print(distanceAbleToDodge + "" + dodgePosition);

        playerState = STATE.ROLLING;

        iTween.MoveTo(gameObject, iTween.Hash("position", dodgePosition, "time", 0.2f, "easeType", "easeInOutExpo", "oncomplete", "onDodgeComplete"));

        // Increase character movespeed
        currMoveSpeed = Mathf.Clamp(currMoveSpeed + rollingMoveSpeed, minMoveSpeed, maxMoveSpeed);
        yield return endOfFrame;

        _dodging = null;
    }

    void Start()
    {
        transform = gameObject.GetComponent<Transform>();
        if (transform == null)
            throw new System.Exception("transform null");
    }

    // Update is called once per frame
    void Update()
    {

        ProcessInputs();

        ProcessAnimations();

    }

    void FixedUpdate()
    {
        Move();
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
        }

    }
  
    void ProcessAnimations()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        animator.SetFloat("Direction", directionFacing);
        animator.SetBool("isRunning", currMoveSpeed > runningMoveSpeed);

        if (playerState == STATE.ROLLING)
        {
            animator.Play("Player_Roll");
            playerState = STATE.NORMAL;
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
        else if(directionFacing == leftDirection && movement.x > 0)
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
                _dodging = StartCoroutine(DodgeCoroutine());
                timeSinceLastRolled = Time.fixedTime;
            }
            else if (Input.GetKeyDown("w"))
            {
                // Jump
                print("W down");
            }
            else if (Input.GetKeyDown("e"))
            {
                // interact
                print("e down");
            }
        }



    }
}
