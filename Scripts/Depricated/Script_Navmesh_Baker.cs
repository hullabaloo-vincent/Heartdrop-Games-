using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_Navmesh_Baker : MonoBehaviour {

    private bool doneBuilding;
    public void BuildLevel() {
        GameObject tileList = GameObject.FindGameObjectWithTag("Generate");
        tileList.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public bool isFinished() {
        return doneBuilding;
    }
}