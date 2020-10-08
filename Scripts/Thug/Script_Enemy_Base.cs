using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_Enemy_Base : MonoBehaviour{
    private NavMeshAgent agent;
    GameObject player;
    Animator playerAnim;

    Dictionary<string, float> movementSpeeds; //collection of movment speed data

    GameObject threatObj;
    bool isUnderThreat = false;
    bool isBlocking = false;

    System.Type enemyType;

    void Start(){

        //Get custom enemy script, which will always be the third element
        Component[] enemyComponents = gameObject.GetComponents(typeof(Component));
        enemyType = enemyComponents[2].GetType();

        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1.6f;
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnim = player.GetComponent<Animator>();

        movementSpeeds = new Dictionary<string, float>(); //set movement speeds
        movementSpeeds.Add("walking", 1f);
        movementSpeeds.Add("running", 3f);
    }

    public void recieveDamage(float damage){
        if (enemyType.ToString() == "Script_Enemy_Thug"){
            gameObject.GetComponent<Script_Enemy_Thug>().recieveDamage(damage);
        }
    }

    public GameObject playerReferece(){
        return player;
    }

    public void move() {
        agent.speed = movementSpeeds["running"];
        agent.SetDestination(player.transform.position);
    }

    public void stopMoving() {
        agent.isStopped = true;
    }

    public Vector3 getAgentVelocity() {
        return agent.velocity;
    }

    public float getAngularSpeed() {
        return agent.angularSpeed;
    }

    /* Tells enemy that a projectile threat will hit it */
    public void threat(GameObject obj) {
        // Only give the ability to block if the enemy is aware
        threatObj = obj;
        isUnderThreat = true;
    }

    public void death() {
        Destroy(gameObject);
    }

    public Vector3 getPlayerLocation() {
        return player.transform.position;
    }
    
    public bool isPlayerSneaking() {
        return false;
    }

    public bool isPlayerWalking() {
        return playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Walk");
    }

    public bool isPlayerRunning() {
        return playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Run");
    }

    /* Returns whether or not the player is in the enemie's line of sight*/
    public bool canSeePlayer() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = player.transform.position - transform.position;

        //-1 = behind; 0 = perpendicular; 1 = front
        if (Vector3.Dot(forward, toOther) > 0.2) {
            return true;
        } else {
            return false;
        }
    }
}
