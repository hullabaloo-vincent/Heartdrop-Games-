using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_Enemy_Base : MonoBehaviour{
    private NavMeshAgent agent;
    GameObject player;
    private Script_Player_Movement PlayerScript;
    Animator playerAnim;

    Dictionary<string, float> movementSpeeds; //collection of movment speed data

    GameObject threatObj;
    bool isUnderThreat = false;
    bool isBlocking = false;

    List<GameObject> Team;

    System.Type enemyType;

    GameObject ParentSpawn;

    void Start(){
        //Init team list
        Team = new List<GameObject>();

        //Get custom enemy script, which will always be the third element
        Component[] enemyComponents = gameObject.GetComponents(typeof(Component));
        enemyType = enemyComponents[2].GetType();

        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 1.6f;

        //Get player references
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = player.GetComponent<Script_Player_Movement>();
        playerAnim = player.GetComponent<Animator>();

        //Set movement speeds
        movementSpeeds = new Dictionary<string, float>();
        movementSpeeds.Add("walking", 1f);
        movementSpeeds.Add("running", 3f);
    }

    //Get the Enemy Spawn gameobject for the room
    public void SetSpawnActor(GameObject parent){
        ParentSpawn = parent;
    }
    public GameObject GetSpawnActor(){
        return ParentSpawn;
    }

    public void SetTeam(List<GameObject> t){
        List<GameObject> temp = new List<GameObject>(t);
        //remove self from team list
        temp.Remove(gameObject);
        Team = temp;
    }

    public List<GameObject> getTeam(){
        return Team;
    }

    public void RemoveFromTeam(GameObject go){
        Team.Remove(go);
    }

    public void AlertTeam(){
        if (enemyType.ToString() == "Script_Enemy_Thug"){
            //gameObject.GetComponent<Script_Enemy_Thug>().
        }
    }

    public void recieveDamage(float damage){
        if (enemyType.ToString() == "Script_Enemy_Thug"){
            gameObject.GetComponent<Script_Enemy_Thug>().RecieveDamage(damage);
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

    public GameObject getPlayerFocus(){
        return PlayerScript.focusedEnemy();
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
