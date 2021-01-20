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

    public static string MainSceneName = "Michael_Testing";
    public static string ChurchSceneMinigameName = "ChurchMinigame";

    private void Start()
    {
        // Spawn points for main
        spawnPoints.Add(MainSceneName, new Vector3(0, 0, 0));
        spawnPoints.Add(MainSceneName + "To" + ChurchSceneMinigameName, new Vector3(0, 0, 0)); // Spawnpoint after main to church
        spawnPoints.Add(ChurchSceneMinigameName + "To" + MainSceneName, new Vector3(-33, 31, 0)); // Spawnpoint after church to main
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
        StartCoroutine(loadSceneMinigameCoroutine(sceneName));
    }

    IEnumerator loadSceneMinigameCoroutine(string sceneName)
    {
        // Remove controls from player...
        PlayerState playerState = gameObject.GetComponent<PlayerState>();
        playerState.setStop();

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

        // Restore controls after we done
        playerState.setNormal();
        Debug.Log("restore controls");
        StartCoroutine(run(sceneName));
    }

    IEnumerator run(string sceneName)
    {
        Debug.Log("running!");
        PlayerState playerState = gameObject.GetComponent<PlayerState>();
        Color objectColor = sceneTransition.GetComponent<Image>().color;
        if (sceneName.Equals(MainSceneName))
        {
            Debug.Log("Running code for main scene");
        }
        else if (sceneName.Equals(ChurchSceneMinigameName))
        {
            playerState.setChurchMinigame();

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
                // Disable player camera + player ui
                GameObject player = GameObject.Find("Player");
                player.transform.Find("PlayerUI").gameObject.SetActive(false);

                
                // TODO FADE Into the new SCENE
                while (sceneTransition.GetComponent<Image>().color.a > 0)
                {
                    Debug.Log("Yello" + objectColor.a);
                    float fadeAmount = Mathf.Max(objectColor.a - (1 * Time.deltaTime), 0);

                    objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                    sceneTransition.GetComponent<Image>().color = objectColor;
                    yield return null;
                }

                churchUI.GetComponent<ChurchMinigame>().run();

                Debug.Log("After animation");
            } // End of church ui check



        }

        // TODO FADE TO SCENE
        while (sceneTransition.GetComponent<Image>().color.a > 0)
        {
            Debug.Log("Yello" + objectColor.a);
            float fadeAmount = Mathf.Max(objectColor.a - (1 * Time.deltaTime), 0);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            sceneTransition.GetComponent<Image>().color = objectColor;
            yield return null;
        }
    }

}
