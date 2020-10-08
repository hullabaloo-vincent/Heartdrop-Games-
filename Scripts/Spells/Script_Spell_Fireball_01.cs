using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Spell_Fireball_01 : MonoBehaviour {
    public float destroy_time = 3f;
    public float damage = 2.5f;
    
    Rigidbody rd;

    void Start() {
        rd = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject, destroy_time);
    }

    void Update() {
        rd.AddForce(transform.forward * 10f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Enemy") {
            other.gameObject.GetComponentInParent<Script_Enemy_Base>().recieveDamage(damage);
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "CollidableObstacles"){
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Default"){
            Destroy(gameObject);
        }
    }
}
