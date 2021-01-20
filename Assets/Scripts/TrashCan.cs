using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour, Interactable
{
    public GameObject gameUI;
    public GameObject expApostolicWork;

    public void Interact(GameObject player)
    {
        float amountOfExp = expApostolicWork.GetComponent<ExpBar>().getMaxExp() / 5;
        expApostolicWork.GetComponent<ExpBar>().addExp(amountOfExp);
        gameUI.GetComponent<CleanUpUI>().dumpTrash();
    }

    public void onAnimationEnd()
    {
        throw new System.NotImplementedException();
    }

    public void onAnimationEvent(GameObject player, InteractableAction action)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
