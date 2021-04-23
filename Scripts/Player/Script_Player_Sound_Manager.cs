using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Sound_Manager : MonoBehaviour
{
    public void PlayFootstep() {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Footsteps/Footsteps_Concrete", gameObject);
    }
}
