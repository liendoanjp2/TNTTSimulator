using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private STATE playerState = STATE.NORMAL;

    private void Start()
    {
        
    }

    private void Update()
    {
      
    }

    public enum STATE
    {
        NORMAL, INTERACTING, STOP
    }

    public void setNormal()
    {
        playerState = STATE.NORMAL;
    }

    public void setInteracting()
    {
        playerState = STATE.INTERACTING;
    }

    public void setStop()
    {
        playerState = STATE.STOP;
    }

    public STATE getPlayerState()
    {
        return playerState;
    }
}
