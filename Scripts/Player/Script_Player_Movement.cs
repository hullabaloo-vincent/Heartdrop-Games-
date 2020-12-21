using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Movement : MonoBehaviour
{
    void Start()
    {
        _AttackCombo = new List<List<string>>();
        _AttackCombo.Add(new List<string> { "punch1", "punch2", "punch3" }); //Combo 1
        _AttackCombo.Add(new List<string> { "punch4" }); //Combo 2
        _AttackCombo.Add(new List<string> { "punch7", "punch8" }); //Combo 3
        _Anim = GetComponent<Animator>();
        _Rd = GetComponent<Rigidbody>();

        InitSpells();

        //iso controls
        Forward = Camera.main.transform.forward;
        Forward.y = 0;
        Forward = Vector3.Normalize(Forward);
        Right = Quaternion.Euler(new Vector3(0, 90, 0)) * Forward;
        _MainCamera = Camera.main;

        //define movement speeds
        _MovementSpeeds = new Dictionary<string, float>(); //set movement speeds
        _MovementSpeeds.Add("walking", 1f);
        _MovementSpeeds.Add("running", 4f);
        _MovementSpeeds.Add("dashing", 6f);
        _MovementSpeeds.Add("attackPunch", 8f);
        _MovementSpeeds.Add("heavyRecoil", 14f);
        _MovementSpeeds.Add("lightRecoil", 8f);

        //map coordinates of player limbs
        _RightHand = gameObject.transform.Find(
            "PC_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand"
            ).gameObject;
        _LeftHand = gameObject.transform.Find(
            "PC_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand"
            ).gameObject;
    }

    private void InitSpells()
    {
        _SpellSlot1 = SpellController.slots[0].GetComponent<Animator>();
        _SpellSlot2 = SpellController.slots[1].GetComponent<Animator>();
        _SpellSlot3 = SpellController.slots[2].GetComponent<Animator>();

        //get current selected spell from active spell slots
        if (SpellController.activeSlots[0])
        {
            foreach (AnimatorControllerParameter parameter in _SpellSlot1.parameters)
            {
                if (_SpellSlot1.GetBool(parameter.name))
                {
                    _Spell1 = parameter.name;
                    break;
                }
            }
        }
        if (SpellController.activeSlots[1])
        {
            foreach (AnimatorControllerParameter parameter in _SpellSlot2.parameters)
            {
                if (_SpellSlot2.GetBool(parameter.name))
                {
                    _Spell2 = parameter.name;
                    break;
                }
            }
        }
        if (SpellController.activeSlots[2])
        {
            foreach (AnimatorControllerParameter parameter in _SpellSlot3.parameters)
            {
                if (_SpellSlot3.GetBool(parameter.name))
                {
                    _Spell3 = parameter.name;
                    break;
                }
            }
        }
    }

    void Update()
    {
        Move();
        #region Movement
        //if player is not currently dashing, and the player is pressing down on the movement keys
        if (!_Anim.GetBool("isDashing") && !_Anim.GetBool("isDrinking") && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftShift)))
        {
        }
        else
        {
            //no movement keys are pressed. Make sure movment bools are set to false
            _Anim.SetBool("isWalking", false);
            _Anim.SetBool("isRunning", false);
            _Rd.velocity = new Vector3(0, 0, 0);
        }

        //dash if player is pressing space, already moving, and not currently dashing
        if (Input.GetKey(KeyCode.Space) && _CanDash &&
        (Input.GetAxis("HorizontalKey") != 0 || Input.GetAxis("VerticalKey") != 0))
        {
            PostUpdate.UpdateMotionBlur(360f);
            resetAnimation();
            _Anim.SetBool("isDashing", true);
            bool selectedDash = false;
            if (Input.GetAxis("HorizontalKey") == 1 && !selectedDash)
            {
                // anim.SetBool("dashRight", true);
                _Anim.SetBool("dashForward", true);
                selectedDash = true;
                _DashKeyName = "horizontal";
                _DashKeyValue = 1;
            }
            if (Input.GetAxis("HorizontalKey") == -1 && !selectedDash)
            {
                //anim.SetBool("dashLeft", true);
                _Anim.SetBool("dashForward", true);
                selectedDash = true;
                _DashKeyName = "horizontal";
                _DashKeyValue = -1;
            }
            if (Input.GetAxis("VerticalKey") == 1 && !selectedDash)
            {
                _Anim.SetBool("dashForward", true);
                selectedDash = true;
                _DashKeyName = "vertical";
                _DashKeyValue = 1;
            }
            if (Input.GetAxis("VerticalKey") == -1 && !selectedDash)
            {
                _Anim.SetBool("dashBack", true);
                selectedDash = true;
                _DashKeyName = "vertical";
                _DashKeyValue = -1;
            }
        }
        #endregion
        #region Attacking and blocking
        //attacking
        if (Input.GetMouseButtonDown(0))
        {
            //666 = no spell selected, therefore the player will use melee
            if (SpellController.CurrentlySelected() == 666 && !_Anim.GetBool("isPunching"))
            {
                resetAnimation();
                _Anim.SetBool("isPunching", true);
                int Chain = Mathf.Clamp(_ChainLevel, _MinChain, _MaxChain);
                _Anim.SetBool(_AttackCombo[Chain][Random.Range(0, _AttackCombo[Chain].Count)], true);
            }
            else
            {
                if (!_ActivatedSpell && !_Anim.GetBool("isDrinking") && _CanCast)
                {
                    resetAnimation();
                    Debug.Log("DRINKIN' ON THE JOB ARE YE");
                    _Anim.SetBool("isDrinking", true);
                }
                if (_ActivatedSpell && !_Anim.GetBool("isCasting"))
                {
                    resetAnimation();
                    _Anim.SetBool("isCasting", true);
                }
            }
        }

        //blocking
        if (Input.GetMouseButtonDown(1))
        {
            resetAnimation();
            _Anim.SetBool("isBlocking", true);
        }

        //if player is too far away from selected object, turn off marker
        if (_EnemySelection != null)
        {
            if (Vector3.Distance(_EnemySelection.transform.position, gameObject.transform.position) > 5f)
            {
                //Tell the enemy that the player is no longer focusing on it
                _EnemySelection.GetComponent<Script_Enemy_Base>().StopPlayerFocus();
                _SelectedEnemy = false;
                _EnemySelection = null;
            }
        }

        //spell selection
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //666 = no spell selected
            if (SpellController.CurrentlySelected() == 666)
            {
                SpellController.SelectSpell(1);
                _ActiveSpell = _Spell1;
                return;
            }
            else
            {
                SpellController.DeselectSlot(1);
                return;
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        _Rd.velocity = _InputVector;

        if (!_Anim.GetBool("isDrinking") && !_Anim.GetBool("isPunching"))
        {
            RotateActor(); //rotate actor towards mouse
        }
    }

    void Move()
    {
        float tempMoveSpeed = 0; //temporary move speed that will assume new movement values

        if (!_Anim.GetBool("isPunching") && !_Anim.GetBool("isDashing") && !_Anim.GetBool("isDrinking") &&
         (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftShift)))
        {
            resetAnimation();
            if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
            {
                tempMoveSpeed = _MovementSpeeds["running"];

                _Anim.SetBool("isRunning", true);
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
                {
                    tempMoveSpeed = _MovementSpeeds["walking"];
                    if (_SelectedEnemy)
                    {
                        _Anim.SetBool("isSelectedEnemy", true);
                    }
                    _Anim.SetBool("isWalking", true);
                }
            }
        }

        if (_Anim.GetBool("isDashing"))
        {
            tempMoveSpeed = _MovementSpeeds["dashing"];
        }

        //move character
        Vector3 rightMovement = Right * Input.GetAxisRaw("Horizontal") * tempMoveSpeed;
        Vector3 upMovement = Forward * Input.GetAxisRaw("Vertical") * tempMoveSpeed;
        Vector3 playerMovement = (rightMovement + upMovement);

        //moveCharacter if player only if they are using the walking or running animation
        if (_Anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") ||
        _Anim.GetCurrentAnimatorStateInfo(0).IsName("WalkCombat") ||
        _Anim.GetCurrentAnimatorStateInfo(0).IsName("WalkLeft") ||
        _Anim.GetCurrentAnimatorStateInfo(0).IsName("WalkRight") ||
        _Anim.GetCurrentAnimatorStateInfo(0).IsName("WalkBack") ||
        _Anim.GetCurrentAnimatorStateInfo(0).IsName("Run")
        && (!_Anim.GetBool("isPunching") &&
        !_Anim.GetBool("tookDamage")))
        {
            _InputVector = playerMovement;
        }
    }
    void RotateActor()
    {
        if (_SelectedEnemy)
        {
            Vector3 lookPos = _EnemySelection.transform.position;
            lookPos.y = 0;
            Vector3 targetDirection = _EnemySelection.transform.position - transform.position;
            targetDirection.y = transform.position.y;
            float singleStep = RotationDamping * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            _Rd.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            //8-way rotation based on WASD keys
            Vector3 angles = transform.eulerAngles;
            if (Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") == 0)
            {
                angles = new Vector3(0f, 50f, 0f); //West
                _Anim.SetBool("walkForward", true);
            }
            if (Input.GetAxisRaw("Horizontal") == -1 && Input.GetAxisRaw("Vertical") == 0)
            {
                angles = new Vector3(0f, 240f, 0f); //East
                _Anim.SetBool("walkForward", true);
            }
            if (Input.GetAxisRaw("Vertical") == 1 && Input.GetAxisRaw("Horizontal") == 0)
            {
                angles = new Vector3(0f, -45f, 0f); //North
                _Anim.SetBool("walkForward", true);
            }
            if (Input.GetAxisRaw("Vertical") == -1 && Input.GetAxisRaw("Horizontal") == 0)
            {
                angles = new Vector3(0f, 130f, 0f); //South
                _Anim.SetBool("walkForward", true);
            }

            if (Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") == 1)
            {
                angles = new Vector3(0f, -28f, 0f); //NorthEast
                _Anim.SetBool("walkForward", true);
            }
            if (Input.GetAxisRaw("Horizontal") == 1 && Input.GetAxisRaw("Vertical") == -1)
            {
                angles = new Vector3(0f, 85f, 0f); //SouthEast
                _Anim.SetBool("walkForward", true);
            }
            if (Input.GetAxisRaw("Horizontal") == -1 && Input.GetAxisRaw("Vertical") == 1)
            {
                angles = new Vector3(0f, -94f, 0f); //NorthWest
                _Anim.SetBool("walkForward", true);
            }
            if (Input.GetAxisRaw("Horizontal") == -1 && Input.GetAxisRaw("Vertical") == -1)
            {
                angles = new Vector3(0f, 156f, 0f); //SouthWest
                _Anim.SetBool("walkForward", true);
            }
            _MovementDirection = angles;
            float turningRate = 200f;
            _Rd.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(angles), turningRate * Time.deltaTime);
        }
    }

    public void StopFocus()
    {
        //Tell the enemy that the player is no longer focusing on it
        _EnemySelection.GetComponent<Script_Enemy_Base>().StopPlayerFocus();
        _SelectedEnemy = false;
        _EnemySelection = null;
    }

    public GameObject FocusedEnemy()
    {
        return _EnemySelection;
    }
    /* This function is only called by enemies. _enemy value is passed through by an enemy class*/
    public void SetFocusEnemy(GameObject _enemy)
    {
        _EnemySelection = _enemy;
        _SelectedEnemy = true;
    }

    //Called via animation event from Anim_Player_Punch
    public void punchRight()
    {
        //Instantiate punch volume at player's right hand
        GameObject playerHit = Instantiate(PunchObj, _RightHand.transform.position, _RightHand.transform.rotation);
        //Ignore collisions from instantiated gameobject
        Physics.IgnoreCollision(playerHit.GetComponent<BoxCollider>(), GetComponent<CapsuleCollider>());
        //Set spawn reference to player
        playerHit.GetComponent<Script_Player_Punch>().SetPlayerReference(gameObject);
    }
    public void punchLeft()
    {
        //Instantiate punch volume at player's left hand
        GameObject playerHit = Instantiate(PunchObj, _LeftHand.transform.position, _LeftHand.transform.rotation);
        //Ignore collisions from instantiated gameobject
        Physics.IgnoreCollision(playerHit.GetComponent<BoxCollider>(), GetComponent<CapsuleCollider>());
        //Set spawn reference to player
        playerHit.GetComponent<Script_Player_Punch>().SetPlayerReference(gameObject);
    }

    public void HitLanded()
    {
        //If a chain is already started, stop the ongoing countdown to reset it
        if (_CanChain)
        {
            StopCoroutine("HitChain");
        }
        _CanChain = true; //Is in chain
        _ChainLevel++; //Current chain
        StartCoroutine("HitChain"); //Restart chain
        Debug.Log("Chain Level: " + _ChainLevel);
    }

    //Resets chain
    IEnumerator HitChain()
    {
        yield return new WaitForSeconds(1f);
        _CanChain = false; //Turn of chain
        _ChainLevel = 0; //Reset chain value
        Debug.Log("Stopping Chain");
        yield return 0;
    }

    private void AttackForce()
    {
        float strength = _MovementSpeeds["attackPunch"];
        //Move the player a tiny bit forwared with an attack 
        _Rd.MovePosition(_Rd.position + transform.forward * strength * Time.deltaTime);
    }

    //recieve damage from enemies
    public void RecieveDamage(float damage, GameObject attack)
    {
        if (!_Anim.GetBool("blockSafe") && _CanTakeDamge && !_Anim.GetBool("inDamage"))
        {
            resetAnimation();
            Health -= damage;
            resetAnimation();
            if (damage >= 0.4)
            {
                _Anim.SetBool("tookDamage_heavy", true);
                //Move player backwards
                _Rd.MovePosition(_Rd.position + -transform.forward * _MovementSpeeds["heavyRecoil"] * Time.deltaTime);
            }
            else
            {
                _Anim.SetBool("tookDamage_light", true);
                _Rd.MovePosition(_Rd.position + -transform.forward * _MovementSpeeds["lightRecoil"] * Time.deltaTime);
            }
            PostUpdate.UpdateChromaticAberration(1.0f);
        }
    }
    private void resetAnimation()
    {
        foreach (AnimatorControllerParameter parameter in _Anim.parameters)
        {
            _Anim.SetBool(parameter.name, false);
        }
    }
    private void SetIdle()
    {
        if (_SelectedEnemy)
        {
            _Anim.SetBool("isSelectedEnemy", true);
        }
        _Anim.SetBool("isIdle", true);
    }
    #region Dashing Animation controls
    private void DashStart()
    {
        _CanDash = false;
    }
    private void DashEnd()
    {
        resetAnimation();
        SetIdle();
        _Rd.velocity = Vector3.zero;
        StartCoroutine("DashCooldown");
    }

    private void invincibilityStart()
    {
        _CanTakeDamge = false;
    }
    private void invincibilityEnd()
    {
        _CanTakeDamge = true;
    }
    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(1.5f);
        _CanDash = true;
        yield return 0;
    }
    #endregion
    #region Blocking Animation Controls
    public void BlockStart()
    {
        resetAnimation();
        _Anim.SetBool("blockSafe", true);
    }

    public void BlockEnd()
    {
        _Anim.SetBool("blockSafe", false);
        _Anim.SetBool("blockEnd", true);
        SetIdle();
    }
    #endregion
    #region Punch Animation Controls
    public void startPunch()
    {
        _IsPunching = true;
        _Anim.SetBool("inPunch", true);
    }
    public void endPunch()
    {
        _IsPunching = false;
        resetAnimation();
        _Anim.SetBool("attackCooldown", true);
    }
    #endregion
    #region Spell Animation Controls
    public void drinkEnd()
    {
        _ActivatedSpell = true;
        _Anim.SetBool("isDrinking", false);
        if (SpellController.CurrentlySelected() == 0)
        {
            StartCoroutine("SpellCoolDownSlot1");
        }

        if (SpellController.CurrentlySelected() == 1)
        {
            StartCoroutine("SpellCoolDownSlot2");
        }

        if (SpellController.CurrentlySelected() == 2)
        {
            StartCoroutine("SpellCoolDownSlot3");
        }
        resetAnimation();
        SetIdle();
    }
    public void castEnd()
    {
        resetAnimation();
        SetIdle();
    }
    public void castSpell()
    {
        string spellName = "VFX/Spells/Spell_" + SpellController.ActiveSpell();
        GameObject spellToShoot = Resources.Load<GameObject>(spellName);
        GameObject spellPrefab = Instantiate(spellToShoot, _RightHand.transform.position, transform.rotation) as GameObject;
    }
    IEnumerator SpellCoolDownSlot1()
    {
        yield return new WaitForSeconds(15f);
        _CanCast = false;
        _ActivatedSpell = false;
        StartCoroutine("CastingCooldown");
        yield return 0;
    }
    IEnumerator SpellCoolDownSlot2()
    {
        yield return new WaitForSeconds(15f);
        _CanCast = false;
        _ActivatedSpell = false;
        StartCoroutine("CastingCooldown");
        yield return 0;
    }
    IEnumerator SpellCoolDownSlot3()
    {
        yield return new WaitForSeconds(15f);
        _CanCast = false;
        _ActivatedSpell = false;
        StartCoroutine("CastingCooldown");
        yield return 0;
    }
    IEnumerator CastingCooldown()
    {
        yield return new WaitForSeconds(60f);
        _CanCast = true;
        yield return 0;
    }
    #endregion
    #region Attack To Idle Animation Controls
    public void endAttackIdle()
    {
        resetAnimation();
    }
    #endregion
    #region Damage Animation Controls
    public void turnOffDamage()
    {
        resetAnimation();
        SetIdle();
    }
    public void damageStart()
    {
        resetAnimation();
        _Anim.SetBool("inDamage", true);
        _Anim.SetBool("tookDamage", true);
    }
    #endregion

    private Camera _MainCamera;
    private Animator _Anim;
    private Rigidbody _Rd;

    public Script_Post_Update PostUpdate;
    public Script_Spell_System SpellController;
    private Animator _SpellSlot1;
    private Animator _SpellSlot2;
    private Animator _SpellSlot3;

    private string _Spell1;
    private string _Spell2;
    private string _Spell3;

    private string _ActiveSpell;

    private GameObject _DebugObj; //for cursor

    private Dictionary<string, float> _MovementSpeeds; //collection of movment speed data

    private Vector3 Forward, Right;

    private Vector3 _InputVector;
    const float RotationDamping = 5.5f;
    private bool _IsDead = false;
    private bool _CanTakeDamge = true;
    public float Health = 1;
    private bool _IsPunching = false;
    private bool _CanDash = true;
    private bool _SelectedEnemy = false;
    private GameObject _EnemySelection;
    private GameObject _EnemyMarker;
    private string _DashKeyName;
    private int _DashKeyValue;

    private GameObject _RightHand;
    private GameObject _LeftHand;
    public GameObject PunchObj;
    private bool _HasHit = false;
    private bool _CanChain = false;
    private int _ChainLevel = 0;

    private bool _ActivatedSpell = false;
    private bool _CanCast = true;

    private List<List<string>> _AttackCombo;
    private int _MinChain = 0;
    private int _MaxChain = 3;

    private Vector3 _MovementDirection;
}