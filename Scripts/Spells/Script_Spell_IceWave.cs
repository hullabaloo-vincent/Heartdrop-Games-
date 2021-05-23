using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Spell_IceWave : MonoBehaviour
{
    //this spell lingers in an aoe, so it doesn't have destory object conditions
    public float destroy_time = 6f;
    //The damage theoretically ticks each frame, so at 60fps, this spell will deal .5 damage to each target a second. 3 damage total over the six seconds to every target
    public float damage = 0.0083f;

    Rigidbody rd;

    void Start()
    {
        rd = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject, destroy_time);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            // force is how forcefully we will push the player away from the enemy.
            float force = 20;
            other.gameObject.GetComponentInParent<Script_Enemy_Base>().RecieveDamage(damage);
            Rigidbody enemyRigidBody = other.gameObject.GetComponent<Rigidbody>();
            enemyRigidBody.MovePosition(enemyRigidBody.position + -other.transform.forward * force * Time.deltaTime);
        }

        if (other.gameObject.tag == "CollidableObstacles")
        {
            
        }

        if (other.gameObject.tag == "Default")
        {
            
        }
    }
}
