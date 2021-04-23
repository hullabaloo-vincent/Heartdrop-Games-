using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_Sublevel_Manager : MonoBehaviour {
    void Start() {
        // Initializes the value of _HasBeenTriggered on game start
        _HasBeenTriggered = false;
    }
    //
    // Summary:
    //      Preforms the level loading based on the player entering a trigger area
     void OnTriggerEnter(Collider other)
    {
        // If the collided object is the player and the box hasn't been triggered before.
        if (other.gameObject.name == "Player")
        {
            Debug.Log(LoadScenes.Length);
            if (LoadScenes.Length > 0) {
                foreach (string scenes in LoadScenes) {
                    if (!SubLevelMaster.GetLoadedScenes().Contains(scenes)) {
                        SubLevelMaster.AddLoadedScene(scenes);
                        StartCoroutine(LoadSceneAsync(LoadSceneMode.Additive, scenes));
                    }
                }
            }
            if (UnloadScenes.Length > 0)
            {
                // Asynchronously unloads the selected scenes.
                foreach (string scenes in UnloadScenes) {
                    if (SubLevelMaster.GetLoadedScenes().Contains(scenes)) {
                        SubLevelMaster.RemoveLoadedScene(scenes);
                        SceneManager.UnloadSceneAsync(scenes);
                    }
                }
            }
        }
    }

    //
    // Summary:
    //      Loads a scene asynconously. It will load a scene only once the scene fully loads.
    //
    // Parameters:
    //   lsm:
    //     The enum value of LoadSceneMode. Either LoadSceneMode.Additive or LoadSceneMode.Single.
    //
    // Returns:
    //     null
    IEnumerator LoadSceneAsync(LoadSceneMode lsm, string scene_to_load) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene_to_load, lsm);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }


    //
    // Summary:
    //      This enum holds the values of the scenes to load
    public string[] LoadScenes;
    //
    // Summary:
    //      This enum holds the values of the scenes to unload
    public string[] UnloadScenes;
    //
    // Summary:
    //      Ensures that once that loading/unloading the scene can happen only once.
    private bool _HasBeenTriggered;

    public CS_Sublevel_Master SubLevelMaster;
 
}