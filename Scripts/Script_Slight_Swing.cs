using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Slight_Swing : MonoBehaviour
{
          float delta = 0.01f;  // Amount to move left and right from the start point
      float speed;
      float direction;
     private Quaternion startPos;
     void Start()
     {
         startPos = transform.rotation;
         speed = Random.Range(0.1f, 0.8f);
         direction = Random.Range(1f, 5f);
     }
     void Update()
     {
         Quaternion a = startPos;
         a.x += direction * (delta * Mathf.Sin(Time.time * speed));
         transform.rotation = a;
     }
}
