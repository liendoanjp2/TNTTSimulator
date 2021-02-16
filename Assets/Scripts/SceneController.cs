using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private GameObject sceneTransition;
    private AsyncOperation sceneAsync;
    private float minTimeLoading = 1;

    private Dictionary<string, Vector3> spawnPoints = new Dictionary<string, Vector3>();

    public static string MainSceneName = "Michael_Testing";
    public static string ChurchSceneMinigameName = "ChurchMinigame";

    private Coroutine sceneSwitching;

    public enum SceneType
    {
        MainScene, ChurchSceneMinigame
    }

    public static string getSceneNameString(SceneType sceneName)
    {
        switch (sceneName)
        {
            case SceneType.MainScene:
                return MainSceneName;
            case SceneType.ChurchSceneMinigame:
                return ChurchSceneMinigameName;
        }

        return null;
    }

    private void Start()
    {
        // Spawn points for main
        spawnPoints.Add(MainSceneName, new Vector3(0, 0, 0));
        spawnPoints.Add(MainSceneName + "To" + ChurchSceneMinigameName, new Vector3(0, 0, 0)); // Spawnpoint after main to church
        spawnPoints.Add(ChurchSceneMinigameName + "To" + MainSceneName, new Vector3(92, 28, 0)); // Spawnpoint after church to main
        sceneTransition = gameObject.transform.parent.Find("PlayerUI").Find("SceneTransition").gameObject;

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getSpawnPoint(string nameKey)
    {
        return spawnPoints[nameKey];
    }

    public void loadSceneMinigame(string sceneName)
    {
        if(sceneSwitching == null)
        {
            sceneSwitching = StartCoroutine(loadSceneMinigameCoroutine(sceneName));
        }
        
    }

    IEnumerator loadSceneMinigameCoroutine(string sceneName)
    {
        // Remove controls from player...
        PlayerState playerState = gameObject.GetComponent<PlayerState>();
        playerState.setStop();

        // Remove camera from player
        GameObject player = GameObject.Find("Player");
        Camera playerCam = player.transform.Find("Camera").GetComponent<Camera>();
        CameraMovement cameraMovement = playerCam.GetComponent<CameraMovement>();
        cameraMovement.stopFollowPlayer();

        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        sceneLoading.allowSceneActivation = false;
        sceneAsync = sceneLoading;
        // Start time
        float startTime = Time.time;

        Color objectColor = sceneTransition.GetComponent<Image>().color;
        // FADE TO BLACK LOADING SCREEN
        while (sceneTransition.GetComponent<Image>().color.a < 1)
        {
            float fadeAmount = objectColor.a + (1 * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            sceneTransition.GetComponent<Image>().color = objectColor;
            yield return null;
        }
        

        //Wait until we are done loading the scene
        while (sceneLoading.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + sceneLoading.progress);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log(sceneLoading.progress + " progress after loading");

        //Activate the Scene
        sceneAsync.allowSceneActivation = true;

        while (!sceneAsync.isDone)
        {
            // wait until it is really finished
            yield return null;
        }

        // Set loaded scene as active
        Scene sceneToLoad = SceneManager.GetSceneByName(sceneName);

        if (!sceneToLoad.IsValid())
        {
            Debug.Log("Invalid scene...");
            yield break;
        }

        string fromSceneName = SceneManager.GetActiveScene().name;
        string nameKey = fromSceneName + "To" + sceneName;
        Debug.Log(nameKey);

        // Move player object over then set the scene
        gameObject.transform.position = getSpawnPoint(nameKey); ;  // copy gameobj over
        SceneManager.MoveGameObjectToScene(gameObject.transform.parent.gameObject, sceneToLoad);
        

        SceneManager.SetActiveScene(sceneToLoad);
        // Then attempt to delete the old scene
        AsyncOperation sceneUnloading = SceneManager.UnloadSceneAsync(fromSceneName);

        //Wait until we are done loading the scene
        while (sceneUnloading.progress < 1f)
        {
            Debug.Log("Unloading scene " + " [][] Progress: " + sceneUnloading.progress);
            yield return new WaitForEndOfFrame();
        }
        Debug.Log(sceneUnloading.progress + " progress after unloading");


        while(Time.time - startTime < minTimeLoading)
        {
            // wait until we have loaded for minTimeLoading or more
            // TODO LOADING SCREEN HERE?
            yield return new WaitForEndOfFrame();
            Debug.Log(Time.time - startTime);
        }

        // Allow camera to follow player
        cameraMovement.startFollowPlayer();
        Debug.Log("restore controls");
        StartCoroutine(run(sceneName));
    }

    IEnumerator run(string sceneName)
    {
        // In this ensure the state is set to the correct one for the players
        // so that they can move/cannot move depending on the situation
        Debug.Log("running!");
        PlayerState playerState = gameObject.GetComponent<PlayerState>();
        Color objectColor = sceneTransition.GetComponent<Image>().color;
        GameObject player = GameObject.Find("Player");
        PlayerUI playerUI = player.transform.Find("PlayerUI").GetComponent<PlayerUI>();

        if (sceneName.Equals(MainSceneName))
        {
            playerUI.showAllRequiredPlayerUI();

            // TODO FADE TO SCENE
            while (sceneTransition.GetComponent<Image>().color.a > 0)
            {
                Debug.Log("Yello" + objectColor.a);
                float fadeAmount = Mathf.Max(objectColor.a - (1 * Time.deltaTime), 0);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                sceneTransition.GetComponent<Image>().color = objectColor;
                yield return null;
            }

            playerState.setNormal();
            Debug.Log("Running code for main scene");
            
        } // end of mainscene if statement
        else if (sceneName.Equals(ChurchSceneMinigameName))
        {

            Debug.Log("Running code for church minigame scene");

            // Find ChurchUI and let them know we are ready!

            GameObject churchUI = GameObject.Find("ChurchUI");

            if(churchUI == null)
            {
                // return back to scene
                loadSceneMinigame(MainSceneName);
                yield break;
            }
            else
            {
                // Hide everything except scenetransition
                playerUI.hideAllExceptSceneTransition();

                // TODO FADE Into the new SCENE
                while (sceneTransition.GetComponent<Image>().color.a > 0)
                {
                    Debug.Log("Yello" + objectColor.a);
                    float fadeAmount = Mathf.Max(objectColor.a - (1 * Time.deltaTime), 0);

                    objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                    sceneTransition.GetComponent<Image>().color = objectColor;
                    yield return null;
                }

                playerState.setChurchMinigame();
                churchUI.GetComponent<ChurchMinigame>().run();

                Debug.Log("After animation");
            } // End of church ui check
        } // End of churchsceneminigame if statement

        sceneSwitching = null;
    }

}
