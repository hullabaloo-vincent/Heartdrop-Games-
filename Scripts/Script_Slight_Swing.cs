using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Slight_Swing : MonoBehaviour
{
    void Start()
    {
        _StartPos = transform.rotation;
        _Speed = Random.Range(0.1f, 0.8f);
        _Direction = Random.Range(1f, 5f);
    }
    void Update()
    {
        Quaternion a = _StartPos;
        a.x += _Direction * (_Delta * Mathf.Sin(Time.time * _Speed));
        transform.rotation = a;
    }

    private float _Delta = 0.01f;  // Amount to move left and right from the start point
    private float _Speed;
    private float _Direction;
    private Quaternion _StartPos;
}
