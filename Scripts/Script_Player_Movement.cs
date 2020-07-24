using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Movement : MonoBehaviour {
    Vector3 forward, right;
    Camera m_MainCamera;
    Animator anim;

    GameObject debugObj; //for cursor

    Dictionary<string, float> movementSpeeds; //collection of movment speed data

    public float rotationDamping = 8f;

    void Start(){
        anim = GetComponent<Animator>();

        forward = Camera.main.transform.forward; 
        forward.y = 0; 
        forward = Vector3.Normalize(forward); 
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        m_MainCamera = Camera.main;

        movementSpeeds = new Dictionary<string, float>(); //set movement speeds
        movementSpeeds.Add("walking", 0.8f);
        movementSpeeds.Add("running", 3f);

        debugObj = GameObject.FindGameObjectWithTag("Debug");
    }

    void Update() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
            Move();
        } else {
            //no movement keys are pressed. Make sure movment bools are set to false
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
        RotateActor(); //rotate actor towards mouse
    }
    void Move() {

        float tempMoveSpeed = 0; //temporary move speed that will assume new movement values

        if (Input.GetKey(KeyCode.LeftShift)) {
            tempMoveSpeed = movementSpeeds["running"];
            anim.SetBool("isRunning", true);
            anim.SetBool("isWalking", false);
        } else {
            tempMoveSpeed = movementSpeeds["walking"];
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", true);
        }
        //move character
        Vector3 rightMovement = right * tempMoveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = forward * tempMoveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");
        transform.position += rightMovement;
        transform.position += upMovement;
        transform.position += rightMovement;
        transform.position += upMovement;
    }
    void RotateActor() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
               // Debug.Log("Ray hit " + hit.collider);
                debugObj.transform.position = hit.point;

                Vector3 lookPos = hit.point - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);
            } else {
              //  Debug.Log("Ray missed block");
            }
    }
}