using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Sound_Test : MonoBehaviour {    
    void Start() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Ambience/AMB_Track_01");
    }
}
