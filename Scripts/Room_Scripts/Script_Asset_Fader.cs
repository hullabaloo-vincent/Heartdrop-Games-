using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Asset_Fader : MonoBehaviour
{
    void Update()
    {
        Material m = GetComponent<Renderer>().material;
        _CurrentColor = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));
        m.SetColor("_Color", _CurrentColor);
    }

    public void SetVisibility(bool state)
    {
        _Visibility = state;
    }

    private Color _VisibleColor = Color.white;
    private Color _InvisibleColor = Color.white;
    private Color _CurrentColor = Color.white;
    private bool _Visibility = true;
}
