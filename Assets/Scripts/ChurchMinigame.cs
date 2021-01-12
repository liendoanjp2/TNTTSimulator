using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurchMinigame : MonoBehaviour, Minigame
{
    public void load(SceneController sceneController)
    {
        sceneController.loadScene("ChurchMinigame");
    }
}
