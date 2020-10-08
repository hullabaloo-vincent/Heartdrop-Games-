using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Minimap : MonoBehaviour{
    public GameObject minimapSM;
    public GameObject minimapLG;

    bool isMapOpen = false;
    void Update(){
        if (Input.GetKeyDown(KeyCode.M)){
            if (!isMapOpen){
                minimapLG.SetActive(true);
                minimapSM.SetActive(false);
                isMapOpen = true;
            }else{
                minimapLG.SetActive(false);
                minimapSM.SetActive(true);
                isMapOpen = false;
            }
        }
    }
}
