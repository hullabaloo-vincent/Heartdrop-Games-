using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_Navigation_Debugger : MonoBehaviour {
    
    public NavMeshAgent DebugAgent;
    private LineRenderer LineRenderer;
    private void Start() {
        LineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    private void Update() {
        if (DebugAgent.hasPath) {
            LineRenderer.positionCount = DebugAgent.path.corners.Length;
            LineRenderer.SetPositions(DebugAgent.path.corners);
            LineRenderer.enabled = true;
        } else {
            LineRenderer.enabled = false;
        }
    }
}
