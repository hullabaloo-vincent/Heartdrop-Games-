using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Enemy_Kick : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, destroy_time);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponentInParent<Script_Player_Movement>().recieveDamage(damage, gameObject);
        }
    }

    public float destroy_time = 0.1f;
    public float damage = 1.5f;
}
