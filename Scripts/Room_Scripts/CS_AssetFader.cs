using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_AssetFader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _ObjState = false;
        _NewState = false;
        _DissolveValue = 0;
        _M = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_NewState != _ObjState) {
            // Fade out
            if (_NewState) {
                Debug.Log("Fading out");
                if (_DissolveValue < 1) {
                    _DissolveValue += 0.02f;
                } else {
                    _DissolveValue = 1f;
                    _ObjState = true;
                }
            }
            // Fade in
            if (!_NewState) {
                Debug.Log("Fading in");
                if (_DissolveValue > 0) {
                    _DissolveValue -= 0.02f;
                } else {
                    _DissolveValue = 0f;
                    _ObjState = false;
                }
            }
            _M.material.SetFloat("D_Value", _DissolveValue);
        }
    }

    public void SetDissolveState(bool state) 
    {
        _NewState = state;
    }

    private Renderer _M;
    private float _DissolveValue;
    private bool _ObjState;
    private bool _NewState;
}
