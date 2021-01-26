using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    List<GameObject> requiredUIGO = new List<GameObject>();

    // Names of required objects that are childs of PlayerUI
    const string expPanelName = "Exp";
    const string sceneTransitionPanelName = "SceneTransition";

    // Start is called before the first frame update
    void Start()
    {
        // Add requiredUI gameobjects here by name
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case expPanelName:
                case sceneTransitionPanelName:
                    requiredUIGO.Add(child.gameObject);
                    break;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void hideAllPlayerUI()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void hideAllExceptSceneTransition()
    {
        foreach (Transform child in transform)
        {
            if(!child.name.Equals(sceneTransitionPanelName))
                child.gameObject.SetActive(false);
        }
    }

    public void showAllRequiredPlayerUI()
    {
        foreach(GameObject go in requiredUIGO)
        {
            go.SetActive(true);
        }
    }

    public void showAllPlayerUI()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
