using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trash : MonoBehaviour, Interactable
{

    public GameObject trashInstance;
    public GameObject cleanUpMiniGameUI;

    public void Interact(GameObject player)
    {
        cleanUpMiniGameUI.SetActive(true);
        if (cleanUpMiniGameUI.GetComponent<CleanUpUI>().pickUpTrash())
        {
            trashInstance.SetActive(false);
        }
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
