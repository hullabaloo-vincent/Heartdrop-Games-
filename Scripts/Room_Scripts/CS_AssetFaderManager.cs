using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_AssetFaderManager : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            foreach (GameObject obj in ObjectsToDissolve) {
                obj.GetComponent<CS_AssetFader>().SetDissolveState(false);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            foreach (GameObject obj in ObjectsToDissolve) {
                obj.GetComponent<CS_AssetFader>().SetDissolveState(true);
            }
        }
    }

    public GameObject[] ObjectsToDissolve;
}
