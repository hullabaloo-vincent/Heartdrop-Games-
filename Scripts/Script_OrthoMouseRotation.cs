using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_OrthoMouseRotation : MonoBehaviour {
  /*
  This script is for determining player rotation from mouse input using an orthographic cameras
  */
    const float RotationDamping = 5.5f;
   
	private void Rotate(){
	    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Ground"); //Filter out all gameobjects in other layers

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            Vector3 lookPos = hit.point;
            lookPos.y = 0; //Make sure player isn't rotated towards the ground
            Vector3 targetDirection = hit.point - transform.position;
            targetDirection.y = transform.position.y;
            float singleStep = RotationDamping * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);
        }
	  }
}
