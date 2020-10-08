using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Walls : MonoBehaviour
{
    public GameObject wallNorth;
    public GameObject wallSouth;
    public GameObject wallEast;
    public GameObject wallWest;

    public GameObject room;
    void Start(){
        Renderer[] wallSouthChildren = wallSouth.GetComponentsInChildren<MeshRenderer>();
        Renderer[] wallNorthChildren = wallNorth.GetComponentsInChildren<MeshRenderer>();
        Renderer[] wallEastChildren = wallEast.GetComponentsInChildren<MeshRenderer>();
        Renderer[] wallWestChildren = wallWest.GetComponentsInChildren<MeshRenderer>();

        Debug.Log("Room " + room.name.ToString() + ": " + room.transform.rotation.eulerAngles.y);
        if (room.transform.rotation.eulerAngles.y == 0) {
            foreach (Renderer r in wallSouthChildren) {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (Renderer r in wallEastChildren) {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        if (room.transform.rotation.eulerAngles.y == 90) {
            foreach (Renderer r in wallSouthChildren) {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (Renderer r in wallEastChildren) {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (Renderer r in wallNorthChildren) {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
        if (room.transform.rotation.eulerAngles.y == 270) {
            foreach (Renderer r in wallSouthChildren) {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            foreach (Renderer r in wallWestChildren) {
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
    }
}
