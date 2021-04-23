using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Random_Particle_Size : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float newScale = Random.Range(0.2f, 0.6f);
        gameObject.transform.localScale = new Vector3(
            newScale,
            newScale,
            newScale
        );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
