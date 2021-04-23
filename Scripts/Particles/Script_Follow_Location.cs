using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Follow_Location : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (!_IsPlayer)
        {
            gameObject.transform.position = _Parent.transform.position;
        }
        else
        {
            gameObject.transform.position = new Vector3(
                _Parent.transform.position.x,
                _Parent.transform.position.y + 2,
                _Parent.transform.position.z
            );
        }
    }

    public void SetFollowActor(GameObject go, bool isPlayer)
    {
        _Parent = go;
        _IsPlayer = isPlayer;
    }

    private GameObject _Parent;
    private bool _IsPlayer = false;
}
