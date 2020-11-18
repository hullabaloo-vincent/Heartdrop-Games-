using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_Enemy_Base : MonoBehaviour{
    private NavMeshAgent _Agent;
    GameObject _Player;
    private Script_Player_Movement PlayerScript;
    Animator _PlayerAnim;

    Dictionary<string, float> _MovementSpeeds; //collection of movment speed data

    GameObject _ThreatObj;
    bool _isUnderThreat = false;
    bool _isBlocking = false;

    //Team is initialized under SetTeam()
    List<GameObject> Team;

    System.Type _EnemyType;

    GameObject _ParentSpawn;

    private bool _isFlanking;

    float _FlockingRadius = 5;

    private Vector3 _FlankPosition;

    void Start() {

        //Get custom enemy script, which will always be the third element
        Component[] enemyComponents = gameObject.GetComponents(typeof(Component));
        _EnemyType = enemyComponents[2].GetType();

        _Agent = gameObject.GetComponent<NavMeshAgent>();
        _Agent.stoppingDistance = 1.6f;

        //Get player references
        _Player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = _Player.GetComponent<Script_Player_Movement>();
        _PlayerAnim = _Player.GetComponent<Animator>();

        //Set movement speeds
        _MovementSpeeds = new Dictionary<string, float>();
        _MovementSpeeds.Add("walking", 1f);
        _MovementSpeeds.Add("running", 3f);
    }

    //Get the Enemy Spawn gameobject for the room
    public void SetSpawnActor(GameObject _parent){
        _ParentSpawn = _parent;
    }
    public GameObject GetSpawnActor(){
        return _ParentSpawn;
    }

    private Vector3 GetRandomPointBehindPlayer(){
        //Get random point beind player
        Vector3 newLocation = playerReferece().transform.position - (playerReferece().transform.forward * Random.Range(2, 5));
        //Find random point within a radius with newLocation as the center
        Vector3 offset = Random.insideUnitCircle * _FlockingRadius;
	    Vector3 pos = newLocation + offset;
        return pos;
    }

    public bool IsFlanking(){
        return _isFlanking;
    }
    
    public bool FlankStatus(){
        //Check if enemy is currently on a flanking path
        float dist = _Agent.remainingDistance; 
        if (dist <= 1.5) {
            return true;
        }
        return false;
    }

    public void SetFlank(bool _isFlankActive){
        _isFlanking = _isFlankActive;
    }

    public void SetFlankPosition(){
        _FlankPosition = GetRandomPointBehindPlayer();
    }

    /*
    * Is only set if focusedEnemy() in Script_Player_Movement returned null.
    * Should only be fired every time the player has a new focus.
    */
    public void PlayerFocused(){
        //Turn off flank for self
        SetFlank(false);
        foreach (GameObject go in Team){
            //Tell everyone else to flank
            //Set unique flank position
            go.GetComponent<Script_Enemy_Base>().SetFlankPosition();
            //Set flank status to get them moving
            go.GetComponent<Script_Enemy_Base>().SetFlank(true);
        }
        //is under attack
    }

    public void StopPlayerFocus(){
        //Turn off flank for everyone
        foreach (GameObject go in Team){
            //Set flank status to get them moving
            go.GetComponent<Script_Enemy_Base>().SetFlank(false);
        }
    }

    public void SetTeam(List<GameObject> t){
        //Create copy of list 't'
        List<GameObject> temp = new List<GameObject>(t);
        //Remove self from team list
        temp.Remove(gameObject);
        Team = temp;
    }

    public List<GameObject> getTeam(){
        return Team;
    }

    public void RemoveFromTeam(GameObject _go){
        Team.Remove(_go);
    }

    public void AlertTeam(){
        //If the player is focusing on attacking
        if (_EnemyType.ToString() == "Script_Enemy_Thug"){
            //gameObject.GetComponent<Script_Enemy_Thug>().
        }
    }

    public void recieveDamage(float _damage){
        if (_EnemyType.ToString() == "Script_Enemy_Thug"){
            gameObject.GetComponent<Script_Enemy_Thug>().RecieveDamage(_damage);
        }
    }

    public GameObject playerReferece(){
        return _Player;
    }

    public void move() {
        _Agent.speed = _MovementSpeeds["running"];
        /*
        * If flank path is complete, turn off flanking variable and
        * just navigate to player normally
        */
        if (FlankStatus() && _isFlanking){
            SetFlank(false);
        }
        if (_isFlanking){
            //If flank position is too far from Player, reset flanking position
            if (Vector3.Distance(getPlayerLocation(), gameObject.transform.position) > 6){
                SetFlankPosition();
            }
            _Agent.SetDestination(_FlankPosition);
        } else {
            _Agent.SetDestination(_Player.transform.position);
        }
    }

    public void stopMoving() {
        _Agent.isStopped = true;
    }

    public Vector3 getAgentVelocity() {
        return _Agent.velocity;
    }

    public float getAngularSpeed() {
        return _Agent.angularSpeed;
    }

    public GameObject getPlayerFocus(){
        return PlayerScript.focusedEnemy();
    }

    /* Tells enemy that a projectile threat will hit it */
    public void threat(GameObject obj) {
        // Only give the ability to block if the enemy is aware
        _ThreatObj = obj;
        _isUnderThreat = true;
    }

    public void death() {
        Destroy(gameObject);
    }

    public Vector3 getPlayerLocation() {
        return _Player.transform.position;
    }
    
    public bool isPlayerSneaking() {
        return false;
    }

    public bool isPlayerWalking() {
        return _PlayerAnim.GetCurrentAnimatorStateInfo(0).IsName("Walk");
    }

    public bool isPlayerRunning() {
        return _PlayerAnim.GetCurrentAnimatorStateInfo(0).IsName("Run");
    }

    /* Returns whether or not the player is in the enemie's line of sight*/
    public bool canSeePlayer() {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = _Player.transform.position - transform.position;

        //-1 = behind; 0 = perpendicular; 1 = front
        if (Vector3.Dot(forward, toOther) > 0.4) {
            return true;
        } else {
            return false;
        }
    }
}