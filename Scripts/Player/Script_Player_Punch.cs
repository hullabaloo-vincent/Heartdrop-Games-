using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Punch : MonoBehaviour {
    public float destroy_time = 0.1f;
    public float damage = 1.5f;

    void Start() {
        Destroy(gameObject, destroy_time);
    }

    private void OnCollisionEnter(Collision other) {
         // force is how forcefully we will push the player away from the enemy.
         float force = 10;
         // If the object we hit is the enemy
         if (other.gameObject.tag == "Enemy") {
             Debug.Log("Hit");
             // Calculate Angle Between the collision point and the player
             other.gameObject.GetComponentInParent<Script_Enemy_Base>().recieveDamage(damage);
             Vector3 dir = other.contacts[0].point - transform.position;
             // We then get the opposite (-Vector3) and normalize it
             dir = -dir.normalized;
             // And finally we add force in the direction of dir and multiply it by force. 
             // This will push back the player
             GetComponent<Rigidbody>().AddForce(dir*force);
         }
     }
}
