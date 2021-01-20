using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;

    private PlayerState playerState;
    private Coroutine moveToRoutine;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = gameObject.transform.parent.Find("Character").gameObject;
        playerState = player.GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState.getPlayerState() != PlayerState.STATE.CHURCHMINIGAME)
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        }
    }

    public void MoveTo(Vector3 coords, float time)
    {
        if (moveToRoutine == null)
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

}
