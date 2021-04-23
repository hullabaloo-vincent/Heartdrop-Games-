﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Door_Open : MonoBehaviour
{
    void Start()
    {
        _Anim = GetComponent<Animator>();
        _TransitionState = false;
        _InActivationRange = false;
        _PlayerRef = GameObject.FindGameObjectWithTag("Player");
        _NotifRef = GameObject.FindGameObjectWithTag("Notification").GetComponent<Script_UI_Blur>();
    }

    void Update()
    {
        if (_InActivationRange && Input.GetKey(KeyCode.E))
        {
            _NotifRef.SetActiveNotif(false);
            _Anim.SetBool("isOpen", true);
            //WallObj.SetVisibility(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _InActivationRange = true;
            _NotifRef.SetActiveNotif(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _InActivationRange = false;
            _Anim.SetBool("isOpen", false);
            _NotifRef.SetActiveNotif(false);
        }
    }

    public void IsTransitioning()
    {
        _TransitionState = true;
    }
    public void StopTransition()
    {
        _TransitionState = false;
    }

    public Script_Transparency_Individual WallObj;

    private Animator _Anim;
    private GameObject _PlayerRef;
    private Script_UI_Blur _NotifRef;
    private bool _InActivationRange;
    private bool _TransitionState;
}
