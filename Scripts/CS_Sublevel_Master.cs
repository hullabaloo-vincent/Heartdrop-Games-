using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Sublevel_Master : MonoBehaviour
{
    public List<string> LoadedScenes;

    public void Start() {
        LoadedScenes = new List<string>();
    }

    public void AddLoadedScene(string new_scene) {
        LoadedScenes.Add(new_scene);
    }
    public void RemoveLoadedScene(string old_scene) {
        LoadedScenes.Remove(old_scene);
    }
    public List<string> GetLoadedScenes() {
        return LoadedScenes;
    }
}
