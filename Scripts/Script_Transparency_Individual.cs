using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Transparency_Individual : MonoBehaviour
{

    public enum EnumWallType
    {
        North,
        South,
        East,
        West
    }
    public EnumWallType wallType;
    public float DistanceScale;
    public GameObject Marker;
    public Renderer[] Assets;
    GameObject player;
    bool ReverseDirection;
    bool Reset;

    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    private float myDirection;
    [Range(0f, 360f)] private float northDirection;
    private float dif;
    private Direction cardinalDirection;

    void Start()
    {
        ReverseDirection = false;
        Reset = true;
        player = GameObject.FindGameObjectWithTag("Player");
        GetDirection();
    }

    /*void FixedUpdate() {
        if ((cardinalDirection.ToString() == "North" || 
            cardinalDirection.ToString() == "West") && !ReverseDirection && 
            wallType.ToString() == "North"){
            if (Vector3.Distance(Marker.transform.position, player.transform.position) <= DistanceScale){
                ChangeOpacity();
            }
        }
        if (cardinalDirection.ToString() == "South" && 
            ReverseDirection && 
            wallType.ToString() == "South"){
            if (Vector3.Distance(Marker.transform.position, player.transform.position) <= DistanceScale){
                ChangeOpacity();
            }
        }
    }*/

    void Update()
    {
        if (_Visibility &&_TransparencyValue < 1f)
        {
            ChangeOpacity();
        }
        if (!_Visibility &&_TransparencyValue > 0f)
        {
            SetToTransparent();
        }
    }

    public void SetVisibility(bool state)
    {
        _Visibility = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Reset = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!ReverseDirection && Reset)
            {
                SetToTransparent();
                ReverseDirection = true;
                Reset = false;
            }
            else
            {
                if (ReverseDirection && Reset)
                {
                    ReverseDirection = false;
                    Reset = false;
                }
            }
        }
    }

    private void ChangeOpacity()
    {
        foreach (Renderer r in Assets)
        {
            foreach (Material m in r.materials)
            {
                Color col = m.GetColor("_Color");
                _TransparencyValue += _TransparencyOffset;
                col.a = _TransparencyValue;
                m.SetColor("_Color", col);
            }
        }
    }

    private void SetToTransparent()
    {
        foreach (Renderer r in Assets)
        {
            foreach (Material m in r.materials)
            {
                Color col = m.GetColor("_Color");
                _TransparencyValue -= _TransparencyOffset;
                col.a = _TransparencyValue;
                m.SetColor("_Color", col);
            }
        }
    }

    private void GetDirection()
    {
        myDirection = transform.eulerAngles.y;
        northDirection = Input.compass.magneticHeading;

        dif = myDirection - northDirection;
        if (dif < 0) dif += 360f;

        if (dif > 45 && dif <= 135)
        {
            cardinalDirection = Direction.East;
        }
        else if (dif > 135 && dif <= 225)
        {
            cardinalDirection = Direction.South;
        }
        else if (dif > 225 && dif <= 315)
        {
            cardinalDirection = Direction.West;
        }
        else
        {
            cardinalDirection = Direction.North;
        }
    }

    private bool CheckWallStatus()
    {
        if (cardinalDirection.ToString() == "North" && wallType.ToString() == "North")
        {
            return true;
        }
        if (cardinalDirection.ToString() == "North" && wallType.ToString() == "East")
        {
            return true;
        }
        if (cardinalDirection.ToString() == "South" && wallType.ToString() == "South")
        {
            return true;
        }
        if (cardinalDirection.ToString() == "South" && wallType.ToString() == "West")
        {
            return true;
        }
        if (cardinalDirection.ToString() == "East" && wallType.ToString() == "South")
        {
            return true;
        }
        if (cardinalDirection.ToString() == "East" && wallType.ToString() == "East")
        {
            return true;
        }
        if (cardinalDirection.ToString() == "West" && wallType.ToString() == "North")
        {
            return true;
        }
        if (cardinalDirection.ToString() == "West" && wallType.ToString() == "West")
        {
            return true;
        }
        return false;
    }

    private bool _Visibility = true;
    private float _TransparencyValue = 1f;
    private float _TransparencyOffset = 0.001f;
}
