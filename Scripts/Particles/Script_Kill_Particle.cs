using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Kill_Particle : MonoBehaviour
{
    public void Start()
    {
        Destroy(gameObject, ParticleDuration);
    }

    public float ParticleDuration = 1f;
}
