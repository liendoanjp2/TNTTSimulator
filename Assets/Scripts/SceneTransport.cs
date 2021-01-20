using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransport : MonoBehaviour
{
    public SceneController.SceneType sceneName;
    public void load(SceneController sceneController)
    {
        sceneController.loadSceneMinigame(SceneController.getSceneNameString(sceneName));
    }
}
