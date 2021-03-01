using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;

public class NPC : MonoBehaviour, Interactable
{
    public int convoId;

    private void OnCollisionExit2D(GameObject player, Collision2D collision)
    {
        //player.transform.Find("PlayerUI").gameObject.GetComponent.closeText();
    }

    public void Interact(GameObject player)
    {
        //player.GetComponent<PlayerUI>().talkToPlayer();
        throw new System.NotImplementedException();
    }

    public void onAnimationEnd()
    {
        throw new System.NotImplementedException();
    }

    public void onAnimationEvent(GameObject player, InteractableAction action)
    {
        throw new System.NotImplementedException();
    }
}