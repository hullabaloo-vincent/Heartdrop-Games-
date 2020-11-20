using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class Script_Post_Update : MonoBehaviour
{
    void Start()
    {
        _Pp = gameObject.GetComponent<PostProcessVolume>();
        _Pp.profile.TryGetSettings(out _Ca);
        _Pp.profile.TryGetSettings(out _Mb);

        _NewChrom = _DefaultChrom;
        _NewMotionBlur = _DefaultMotionBlur;
    }

    void Update()
    {
        if (_NewChrom > _DefaultChrom)
        {
            _NewChrom -= 0.01f;
        }
        if (_NewMotionBlur > _DefaultMotionBlur)
        {
            _NewMotionBlur -= 1.5f;
        }
        _Ca.intensity.value = _NewChrom;
        _Mb.shutterAngle.value = _NewMotionBlur;
    }

    public void UpdateChromaticAberration(float newValue)
    {
        _NewChrom = newValue;
    }

    public void UpdateMotionBlur(float newValue)
    {
        _NewMotionBlur = newValue;
    }

    private PostProcessVolume _Pp;
    private ChromaticAberration _Ca;
    private MotionBlur _Mb;

    private float _DefaultChrom = 0.034f;
    private float _NewChrom;

    private float _DefaultMotionBlur = 0f;
    private float _NewMotionBlur;
}
