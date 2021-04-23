using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Emissive_Fade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _M = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        _CurrentEmmission = Mathf.Lerp(_MinEmmission, _MaxEmission, Mathf.PingPong(Time.time, 1));
        _M.SetVector("_EmissionColor", new Vector4(0.7490196f,0,0) * _CurrentEmmission);
    }
    
    private Material _M;
    private float _MaxEmission = 4f;
    private float _MinEmmission = 0f;
    private float _CurrentEmmission = 0f;
    private bool _Increase = true;
}
