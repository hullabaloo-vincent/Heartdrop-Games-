using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Spell_Fireball_01 : MonoBehaviour
{
    public float destroy_time = 3f;
    public float damage = 2.5f;

    Rigidbody rd;

    void Start()
    {
        rd = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject, destroy_time);
    }

    void Update()
    {
        rd.AddForce(transform.forward * 10f);
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
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "CollidableObstacles")
        {
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Default")
        {
            Destroy(gameObject);
        }
    }
}
