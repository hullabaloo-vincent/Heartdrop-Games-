using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Punch : MonoBehaviour {
    public float destroy_time = 0.1f;
    public float damage = 1.5f;

    void Start() {
        Destroy(gameObject, destroy_time);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Enemy") {
            other.gameObject.GetComponentInParent<Script_Enemy_Base>().recieveDamage(damage);
            //other.gameObject.GetComponentInParent<Script_Enemy_Thug>().recieveDamage(damage);
        }
    }
}
