using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_Enemy_Base : MonoBehaviour
{
    void Start()
    {
        //Get custom enemy script, which will always be the third element
        Component[] enemyComponents = gameObject.GetComponents(typeof(Component));
        _EnemyType = enemyComponents[2].GetType();

        _Agent = gameObject.GetComponent<NavMeshAgent>();
        _Agent.stoppingDistance = 1.2f;
        _Path = new NavMeshPath();

        _Rd = gameObject.GetComponent<Rigidbody>();
        _LastSquareMagnitude = Mathf.Infinity;

        //Get player references
        _Player = GameObject.FindGameObjectWithTag("Player");
        PlayerScript = _Player.GetComponent<Script_Player_Movement>();
        _PlayerAnim = _Player.GetComponent<Animator>();

        //Set movement speeds
        _MovementSpeeds = new Dictionary<string, float>();
        _MovementSpeeds.Add("walking", 1f);
        _MovementSpeeds.Add("running", 3f);
    }

    private void FixedUpdate()
    {
        _Rd.velocity = _TargetVelocity;
    }

    //Get the Enemy Spawn gameobject for the room
    public void SetSpawnActor(GameObject parent)
    {
        _ParentSpawn = parent;
    }
    public GameObject GetSpawnActor()
    {
        return _ParentSpawn;
    }

    private Vector3 GetRandomPointBehindPlayer()
    {
        //Get random point beind player
        Vector3 newLocation = PlayerReferece().transform.position - (PlayerReferece().transform.forward * Random.Range(2, 5));
        //Find random point within a radius with newLocation as the center
        Vector3 offset = Random.insideUnitCircle * _FlockingRadius;
        Vector3 pos = newLocation + offset;
        return pos;
    }

    public bool IsFlanking()
    {
        return _IsFlanking;
    }

    public bool FlankStatus()
    {
        //Check if enemy is currently on a flanking path
        float dist = _Agent.remainingDistance;
        if (dist <= 1.5)
        {
            return true;
        }
        return false;
    }

    public void SetFlank(bool isFlankActive)
    {
        _IsFlanking = isFlankActive;
    }

    public void SetFlankPosition()
    {
        _CurrentPath = GetRandomPointBehindPlayer();
    }

    /*
    * Is only set if focusedEnemy() in Script_Player_Movement returned null.
    * Should only be fired every time the player has a new focus.
    */
    public void PlayerFocused()
    {
        //Turn off flank for self
        SetFlank(false);
        foreach (GameObject go in _Team)
        {
            //Tell everyone else to flank
            //Set unique flank position
            go.GetComponent<Script_Enemy_Base>().SetFlankPosition();
            //Set flank status to get them moving
            go.GetComponent<Script_Enemy_Base>().SetFlank(true);
        }
        //is under attack
    }

    public void StopPlayerFocus()
    {
        //Turn off flank for everyone
        foreach (GameObject go in _Team)
        {
            //Set flank status to get them moving
            go.GetComponent<Script_Enemy_Base>().SetFlank(false);
        }
    }

    public void SetTeam(List<GameObject> t)
    {
        //Create copy of list 't'
        List<GameObject> temp = new List<GameObject>(t);
        //Remove self from team list
        temp.Remove(gameObject);
        _Team = temp;
    }

    public List<GameObject> GetTeam()
    {
        return _Team;
    }

    public void RemoveFromTeam(GameObject go)
    {
        _Team.Remove(go);
    }

    public void AlertTeam()
    {
        //If the player is focusing on attacking
        if (_EnemyType.ToString() == "Script_Enemy_Thug")
        {
            //gameObject.GetComponent<Script_Enemy_Thug>().
        }
    }

    public void RecieveDamage(float damage)
    {
        if (_EnemyType.ToString() == "Script_Enemy_Thug")
        {
            gameObject.GetComponent<Script_Enemy_Thug>().RecieveDamage(damage);
        }
    }

    public GameObject PlayerReferece()
    {
        return _Player;
    }

    public void RotateEnemy(Vector3 target)
    {
        Vector3 targetDirection = target - transform.position;
        float rotationSpeed = 3;
        float singleStep = rotationSpeed * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public void move()
    {
        _Agent.speed = _MovementSpeeds["running"];

        //if no path exists
        if (_CurrentPath == null)
        {
            _CurrentPath = GetPlayerLocation();
        }
        //If Player deviated from the current path by 1 unit recalculate path
        float distanceFromTarget = Vector3.Distance(_CurrentPath, GetPlayerLocation());
        if (distanceFromTarget > 6.0f)
        {
            //If player is current flanking recalculate the path with a flanking position
            if (_IsFlanking)
            {
                SetFlankPosition();
            }
        } else if (distanceFromTarget >= 1.0f){
            _CurrentPath = GetPlayerLocation();
        }
        /*
        * If flank path is complete, turn off flanking variable and
        * just navigate to player normally
        */
        if (FlankStatus() && _IsFlanking)
        {
            SetFlank(false);
        }

        if (_IsFlanking)
        {
            //Initiate first flank
            if (_FirstFlank)
            {
                SetFlankPosition();
            }
        }
        //Move player to _CurrentPath

        _Agent.SetDestination(_CurrentPath);
    }

    public void stopMoving()
    {
        _Agent.isStopped = true;
    }

    public void UnlockMovement()
    {
        _Agent.isStopped = false;
    }

    public Vector3 GetAgentVelocity()
    {
        return _Agent.velocity;
    }

    public float GetAngularSpeed()
    {
        return _Agent.angularSpeed;
    }

    public GameObject GetPlayerFocus()
    {
        return PlayerScript.FocusedEnemy();
    }

    /* Tells enemy that a projectile threat will hit it */
    public void Threat(GameObject obj)
    {
        // Only give the ability to block if the enemy is aware
        _ThreatObj = obj;
        _IsUnderThreat = true;
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public Vector3 GetPlayerLocation()
    {
        return _Player.transform.position;
    }

    public bool IsPlayerSneaking()
    {
        return false;
    }

    public bool IsPlayerWalking()
    {
        return _PlayerAnim.GetCurrentAnimatorStateInfo(0).IsName("Walk");
    }

    public bool IsPlayerRunning()
    {
        return _PlayerAnim.GetCurrentAnimatorStateInfo(0).IsName("Run");
    }

    /* Returns whether or not the player is in the enemie's line of sight*/
    public bool CanSeePlayer()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = _Player.transform.position - transform.position;

        //-1 = behind; 0 = perpendicular; 1 = front
        if (Vector3.Dot(forward, toOther) > 0.4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSeePlayerNarrow()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = _Player.transform.position - transform.position;

        //-1 = behind; 0 = perpendicular; 1 = front
        if (Vector3.Dot(forward, toOther) > 0.9)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private NavMeshAgent _Agent;
    private NavMeshPath _Path;
    private Vector3 _CurrentPath;
    private bool _FirstFlank;
    const float RotationDamping = 5.5f;
    private Rigidbody _Rd;
    private Vector3 _TargetVelocity;
    private float _LastSquareMagnitude;
    private GameObject _Player;
    private Script_Player_Movement PlayerScript;
    private Animator _PlayerAnim;
    private Dictionary<string, float> _MovementSpeeds; //collection of movment speed data
    private GameObject _ThreatObj;
    private bool _IsUnderThreat = false;
    private bool _IsBlocking = false;
    private List<GameObject> _Team; //Team is initialized under SetTeam()
    private System.Type _EnemyType;
    private GameObject _ParentSpawn;
    private bool _IsFlanking;
    private float _FlockingRadius = 5;
}