using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurchSceneMinigame : MonoBehaviour, SceneMinigame
{
    public void load(SceneController sceneController)
    {
        sceneController.loadSceneMinigame(SceneController.ChurchSceneMinigameName);
    }
}
