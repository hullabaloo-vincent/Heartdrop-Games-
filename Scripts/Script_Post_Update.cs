using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Script_Post_Update : MonoBehaviour
{
    void Start()
    {
        _PostVolume.profile.TryGet<ChromaticAberration>(out _Ca);

        _CurrentChromaticAberration = _MinChromaticAberration;
        _NewMotionBlur = _DefaultMotionBlur;
    }

    void Update()
    {
       if (_CurrentChromaticAberration > _MinChromaticAberration)
        {
            _CurrentChromaticAberration -= 0.01f;
        }

        _Ca.intensity.Override (_CurrentChromaticAberration);
    }

    public void UpdateChromaticAberration(float newValue)
    {
        _CurrentChromaticAberration = newValue;
    }

    public void UpdateMotionBlur(float newValue)
    {
        _NewMotionBlur = newValue;
    }

    public Volume _PostVolume;
    ChromaticAberration _Ca;
    private MotionBlur _Mb;

    private Pixelate _Px;

    private float _MinChromaticAberration = 0.034f;

    private float _CurrentChromaticAberration;

    private float _DefaultMotionBlur = 0f;
    private float _NewMotionBlur;
}
