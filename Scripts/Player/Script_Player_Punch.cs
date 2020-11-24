using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Punch : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }

    public void SetPlayerReference(GameObject p)
    {
        _PlayerRef = p.GetComponent<Script_Player_Movement>();
        _Player = p;
    }

    //Called on when a chain attack is more than 1
    public void UpdateDamage(float newDamage)
    {
        Damage = newDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        // force is how forcefully we will push the player away from the enemy.
        float force = 20;
        // If the object we hit is the enemy
        if (other.gameObject.tag == "Enemy" && !_DoOnce)
        {
            _DoOnce = true;
            _PlayerRef.HitLanded();
            other.gameObject.GetComponentInParent<Script_Enemy_Base>().RecieveDamage(Damage);
            Rigidbody enemyRigidBody = other.gameObject.GetComponent<Rigidbody>();

            enemyRigidBody.MovePosition(enemyRigidBody.position + -other.transform.forward * force * Time.deltaTime);
        }
    }

    public float DestroyTime = 0.35f;
    public float Damage = 1.5f;

    private bool _DoOnce = false;

    private Script_Player_Movement _PlayerRef;

    private GameObject _Player;
}