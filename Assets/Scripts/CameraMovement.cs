using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;

    private PlayerState playerState;
    private SceneController playerSceneController;
    private Coroutine moveToRoutine;

    private bool followPlayer = true;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = gameObject.transform.parent.Find("Character").gameObject;
        playerState = player.GetComponent<PlayerState>();
        playerSceneController = player.GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -10);
        }
    }

    public void MoveTo(Vector3 coords, float time)
    {
        if (moveToRoutine == null && !followPlayer)
        {
            moveToRoutine = StartCoroutine(MoveToCoroutine(coords, time));
        }
        else
        {
            Debug.Log("moveToRoutine still running!");
        }
    }

    private IEnumerator MoveToCoroutine(Vector3 coords, float time)
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", coords, "time", time, "oncomplete", "onMoveToComplete", "easeType", "easeInCubic"));
        yield return new WaitForEndOfFrame();
    }

    private void onMoveToComplete()
    {
        moveToRoutine = null;
        Debug.Log("onMoveToComplete");
    }

    public bool isMoveToOn()
    {
        return moveToRoutine != null ? true : false;
    }

    public void stopFollowPlayer()
    {
        followPlayer = false;
    }

    public void startFollowPlayer()
    {
        followPlayer = true;
    }

}
