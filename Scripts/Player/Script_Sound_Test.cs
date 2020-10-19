using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Sound_Test : MonoBehaviour {
    // Start is called before the first frame update
    
    void Start() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/TestTone");
    }
}
