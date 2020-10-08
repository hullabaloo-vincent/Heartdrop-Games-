using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Arrow_Selection : MonoBehaviour {

    float originalY;

    public float intensity = 0.1f;
     public float frequency = 0.2f;

    void Start (){
        this.originalY = gameObject.transform.position.y;
    }
    void Update() {
        transform.Rotate (0,0,50*Time.deltaTime);

        Vector3 temp = Vector3.up * Mathf.Cos(Time.time * frequency ) * intensity;

        transform.position = new Vector3(transform.position.x, temp.y + originalY, transform.position.z);
    }
}
