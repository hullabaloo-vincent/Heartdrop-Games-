using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Punch : MonoBehaviour {
    public float destroy_time = 0.35f;
    public float damage = 1.5f;

    private bool DoOnce = false;
    
    private Script_Player_Movement PlayerRef;

    void Start() {
        Destroy(gameObject, destroy_time);
    }

    public void SetPlayerReference(GameObject _p) {
        PlayerRef = _p.GetComponent<Script_Player_Movement>();
    }

    //Called on when a chain attack is more than 1
    public void UpdateDamage(float _newDamage){
        damage = _newDamage;
    }

    private void OnCollisionEnter(Collision _other) {
         // force is how forcefully we will push the player away from the enemy.
         float force = 10;
         // If the object we hit is the enemy
         if (_other.gameObject.tag == "Enemy" && !DoOnce) {
             DoOnce = true;
             PlayerRef.HitLanded();
             _other.gameObject.GetComponentInParent<Script_Enemy_Base>().recieveDamage(damage);
             // Calculate Angle Between the collision point and the player
             Vector3 dir = _other.contacts[0].point - transform.position;
             // We then get the opposite (-Vector3) and normalize it
             dir = -dir.normalized;
             // And finally we add force in the direction of dir and multiply it by force. 
             // This will push back the player
             GetComponent<Rigidbody>().AddForce(dir*force);
         }
     }
}
