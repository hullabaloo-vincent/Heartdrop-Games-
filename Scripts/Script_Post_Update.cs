using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class Script_Post_Update : MonoBehaviour {
    PostProcessVolume pp;
    ChromaticAberration ca;
    MotionBlur mb;

    float defaultChrom = 0.034f;
    float newChrom;

    float defaultMotionBlur = 0f;
    float newMotionBlur;

    void Start(){
        pp = gameObject.GetComponent<PostProcessVolume>();
        pp.profile.TryGetSettings(out ca);
        pp.profile.TryGetSettings(out mb);

        newChrom = defaultChrom;
        newMotionBlur = defaultMotionBlur;
    }

    void Update(){
        if (newChrom > defaultChrom) {
            newChrom -= 0.01f;
        }
        if (newMotionBlur > defaultMotionBlur) {
            newMotionBlur -= 1.5f;
        }
        ca.intensity.value = newChrom;
        mb.shutterAngle.value = newMotionBlur;
    }

    public void UpdateChromaticAberration(float newValue) {
        newChrom = newValue;
    }

    public void UpdateMotionBlur(float newValue){
        newMotionBlur = newValue;
    }
}
