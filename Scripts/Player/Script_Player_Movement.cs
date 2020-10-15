using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Movement : MonoBehaviour {
    Camera m_MainCamera;
    Animator anim;
    Rigidbody rd;
    CharacterController controller;

    public Script_Post_Update postUpdate;
    public Script_Spell_System spellController;
    Animator spellSlot1;
    Animator spellSlot2;
    Animator spellSlot3;

    string spell1;
    string spell2;
    string spell3;

    string activeSpell;

    GameObject debugObj; //for cursor

    Dictionary<string, float> movementSpeeds; //collection of movment speed data

    Vector3 forward, right;
    const float RotationDamping = 5.5f;
    bool isDead = false;
    bool canTakeDamge = true;
    public float health = 1;
    bool isPunching = false;
    bool canDash = true;
    bool selectedEnemy = false;
    GameObject enemySelection;
    GameObject enemyMarker;
    string dashKeyName;
    int dashKeyValue;

    GameObject rightHand;
    GameObject leftHand;
    public GameObject punchObj;

    bool activatedSpell = false;
    bool canCast = true;

    void Start() {
        anim = GetComponent<Animator>();
        rd = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();

        spellSlot1 = spellController.slots[0].GetComponent<Animator>();
        spellSlot2 = spellController.slots[1].GetComponent<Animator>();
        spellSlot3 = spellController.slots[2].GetComponent<Animator>();

        //get current selected spell from active spell slots
        if (spellController.activeSlots[0]){
            foreach (AnimatorControllerParameter parameter in spellSlot1.parameters) {
                if (spellSlot1.GetBool(parameter.name)){
                    spell1 = parameter.name;
                    break;
                }
            }
        }
        if (spellController.activeSlots[1]){
            foreach (AnimatorControllerParameter parameter in spellSlot2.parameters) {
                if (spellSlot2.GetBool(parameter.name)){
                    spell2 = parameter.name;
                    break;
                }
            }
        }
        if (spellController.activeSlots[2]){
            foreach (AnimatorControllerParameter parameter in spellSlot3.parameters) {
                if (spellSlot3.GetBool(parameter.name)){
                    spell3 = parameter.name;
                    break;
                }
            }
        }

        //iso controls
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        m_MainCamera = Camera.main;
        
        //define movement speeds
        movementSpeeds = new Dictionary<string, float>(); //set movement speeds
        movementSpeeds.Add("walking", 2f);
        movementSpeeds.Add("running", 6f);
        movementSpeeds.Add("dashing", 6f);

        debugObj = GameObject.FindGameObjectWithTag("Debug");

        //map coordinates of player limbs
        rightHand = gameObject.transform.Find(
            "PC_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand"
            ).gameObject;
        leftHand = gameObject.transform.Find(
            "PC_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand"
            ).gameObject;
    }

    void Update() {
        #region Movement
        //if player is not currently dashing, and the player is pressing down on the movement keys
        if (!anim.GetBool("isDashing") && !anim.GetBool("isDrinking") && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftShift))) {
            Move();
        } else {
            //no movement keys are pressed. Make sure movment bools are set to false
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }

        if (anim.GetBool("isDashing")){
            if (dashKeyName == "horizontal"){
                controller.Move(right * movementSpeeds["dashing"] * Time.deltaTime * dashKeyValue);
                controller.Move(right * movementSpeeds["dashing"] * Time.deltaTime * dashKeyValue);
            } else {
                controller.Move(forward * movementSpeeds["dashing"] * Time.deltaTime * dashKeyValue);
                controller.Move(forward * movementSpeeds["dashing"] * Time.deltaTime * dashKeyValue);
            }        
        }
        
        //dash if player is pressing space, already moving, and not currently dashing
        if (Input.GetKey(KeyCode.Space) && canDash && 
        (Input.GetAxis("HorizontalKey") != 0 || Input.GetAxis("VerticalKey") != 0)){
            postUpdate.UpdateMotionBlur(360f);
            resetAnimation();
            anim.SetBool("isDashing", true);
            bool selectedDash = false;
            if (Input.GetAxis("HorizontalKey") == 1 && !selectedDash){
                anim.SetBool("dashRight", true);
                selectedDash = true;
                dashKeyName = "horizontal";
                dashKeyValue = 1;
            }
            if (Input.GetAxis("HorizontalKey") == -1 && !selectedDash){
                anim.SetBool("dashLeft", true);
                selectedDash = true;
                dashKeyName = "horizontal";
                dashKeyValue = -1;
            }
            if (Input.GetAxis("VerticalKey") == 1 && !selectedDash){
                anim.SetBool("dashForward", true);
                selectedDash = true;
                dashKeyName = "vertical";
                dashKeyValue = 1;
            }
            if (Input.GetAxis("VerticalKey") == -1 && !selectedDash){
                anim.SetBool("dashBack", true);
                selectedDash = true;
                dashKeyName = "vertical";
                dashKeyValue = -1;
            }
        }
        if (!anim.GetBool("isDrinking") && !anim.GetBool("isPunching")){
            RotateActor(); //rotate actor towards mouse
        }
        #endregion
        #region Attacking and blocking
        //attacking
        if (Input.GetMouseButtonDown(0)) {
            //666 = no spell selected, therefore the player will use melee
            if (spellController.currentlySelected() == 666 && !anim.GetBool("isPunching")){
                int punchChooser = Random.Range(1, 5);
                resetAnimation();
                anim.SetBool("isPunching", true);
                switch (punchChooser) {
                    case 1:
                        anim.SetBool("punch1", true);
                        break;
                    case 2:
                        anim.SetBool("punch2", true);
                        break;
                    case 3:
                        anim.SetBool("punch3", true);
                        break;
                    case 4:
                        anim.SetBool("punch4", true);
                        break;
                    case 5:
                        anim.SetBool("punch5", true);
                        break;
                }
            } else {
                if (!activatedSpell && !anim.GetBool("isDrinking") && canCast){
                        resetAnimation();
                        anim.SetBool("isDrinking", true);
                }
                if (activatedSpell && !anim.GetBool("isCasting")){
                    resetAnimation();
                    anim.SetBool("isCasting", true);
                }
            }
        }

        //blocking
        if (Input.GetMouseButtonDown(1)) {
            resetAnimation();
            anim.SetBool("isBlocking", true); 
        }        
        
        //if player is too far away from selected object, turn off marker
        if (enemySelection != null){
            if (Vector3.Distance(enemySelection.transform.position, gameObject.transform.position) > 5f){
                selectedEnemy = false;
                enemySelection = null;
            }
        }

        //spell selection
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            //666 = no spell selected
            if (spellController.currentlySelected() == 666){
                spellController.selectSpell(1);
                activeSpell = spell1;
                return;
            } else {
                spellController.deselectSlot(1);
                return;
            }
        }
        #endregion
    }
    
    void Move() {
        float tempMoveSpeed = 0; //temporary move speed that will assume new movement values
        if (!anim.GetBool("isPunching")) {
            resetAnimation();
            if (Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))) {
                tempMoveSpeed = movementSpeeds["running"];

                anim.SetBool("isRunning", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("attackCooldown", false);
            } else {
                if (!Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))) {
                    tempMoveSpeed = movementSpeeds["walking"];
                    if (selectedEnemy){
                        anim.SetBool("isSelectedEnemy", true);
                    }
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isWalking", true);
                    anim.SetBool("attackCooldown", false);
                }
            }
        }

        //move character
        Vector3 rightMovement = right * tempMoveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = forward * tempMoveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");
        Vector3 playerMovement = rightMovement + upMovement;

        //moveCharacter if player only if they are using the walking or running animation
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") || 
        anim.GetCurrentAnimatorStateInfo(0).IsName("WalkCombat") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("WalkLeft") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("WalkRight") || 
        anim.GetCurrentAnimatorStateInfo(0).IsName("WalkBack") ||
        anim.GetCurrentAnimatorStateInfo(0).IsName("Run") 
        && (!anim.GetBool("isPunching") && 
        !anim.GetBool("tookDamage"))) {
            controller.Move(playerMovement);
        }
    }
    void RotateActor() {
        if (selectedEnemy){
            Vector3 lookPos = enemySelection.transform.position;
            lookPos.y = 0;
            Vector3 targetDirection = enemySelection.transform.position - transform.position;
            targetDirection.y = transform.position.y;
            float singleStep = RotationDamping * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);
        } else { 
            //8-way rotation based on WASD keys
            Vector3 angles = transform.eulerAngles;
            if (Input.GetAxis("HorizontalKey") == 1 && Input.GetAxis("VerticalKey") == 0){
                angles = new Vector3(0f, 50f, 0f); //West
                anim.SetBool("walkForward", false);
                anim.SetBool("walkBack", false);
                anim.SetBool("walkLeft", true);
                anim.SetBool("walkRight", false);
            }
            if (Input.GetAxis("HorizontalKey") == -1 && Input.GetAxis("VerticalKey") == 0){
                angles = new Vector3(0f, 240f, 0f); //East
                anim.SetBool("walkForward", false);
                anim.SetBool("walkBack", false);
                anim.SetBool("walkLeft", false);
                anim.SetBool("walkRight", true);
            }
            if (Input.GetAxis("VerticalKey") == 1 && Input.GetAxis("HorizontalKey") == 0){
                angles = new Vector3(0f, -45f, 0f); //North
                anim.SetBool("walkForward", true);
                anim.SetBool("walkBack", false);
                anim.SetBool("walkLeft", false);
                anim.SetBool("walkRight", false);
            }
            if (Input.GetAxis("VerticalKey") == -1 && Input.GetAxis("HorizontalKey") == 0){
                angles = new Vector3(0f, 130f, 0f); //South
                anim.SetBool("walkForward", true);
                anim.SetBool("walkBack", false);
                anim.SetBool("walkLeft", false);
                anim.SetBool("walkRight", false);
            }

            if (Input.GetAxis("HorizontalKey") == 1 && Input.GetAxis("VerticalKey") == 1){
                angles = new Vector3(0f, -28f, 0f); //NorthEast
                anim.SetBool("walkForward", true);
                anim.SetBool("walkBack", false);
                anim.SetBool("walkLeft", false);
                anim.SetBool("walkRight", false);
            }
            if (Input.GetAxis("HorizontalKey") == 1 && Input.GetAxis("VerticalKey") == -1){
                angles = new Vector3(0f, 85f, 0f); //SouthEast
                anim.SetBool("walkForward", false);
                anim.SetBool("walkBack", true);
                anim.SetBool("walkLeft", false);
                anim.SetBool("walkRight", false);
            }
            if (Input.GetAxis("HorizontalKey") == -1 && Input.GetAxis("VerticalKey") == 1){
                angles = new Vector3(0f, -94f, 0f); //NorthWest
                anim.SetBool("walkForward", true);
                anim.SetBool("walkBack", false);
                anim.SetBool("walkLeft", false);
                anim.SetBool("walkRight", false);
            }
            if (Input.GetAxis("HorizontalKey") == -1 && Input.GetAxis("VerticalKey") == -1){
                angles = new Vector3(0f, 156f, 0f); //SouthWest
                anim.SetBool("walkForward", false);
                anim.SetBool("walkBack", true);
                anim.SetBool("walkLeft", false);
                anim.SetBool("walkRight", false);
            }
            float turningRate = 250f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(angles), turningRate * Time.deltaTime);
        }
    }
    
    public void stopFocus(){
        selectedEnemy = false;
        enemySelection = null;
    }

    public GameObject focusedEnemy(){
        return enemySelection;
    }
    public void setFocusEnemy(GameObject enemy){
        enemySelection = enemy;
        selectedEnemy = true;
    }

    //Called via animation event from Anim_Player_Punch
    public void punchRight() {
        GameObject playerHit = Instantiate(punchObj, rightHand.transform.position, rightHand.transform.rotation);
    }
    public void punchLeft() {
        GameObject playerHit = Instantiate(punchObj, leftHand.transform.position, leftHand.transform.rotation);
    }

    //recieve damage from enemies
    public void recieveDamage(float damage, GameObject attack) {
        if (!anim.GetBool("blockSafe") && canTakeDamge && !anim.GetBool("inDamage")) {
            resetAnimation();
            health -= damage;
            resetAnimation();
            if (damage >= 0.4) {
                anim.SetBool("tookDamage_heavy", true);
                controller.Move((transform.forward * -1) * 10f * Time.deltaTime);
            } else {
                anim.SetBool("tookDamage_light", true);
            }
            postUpdate.UpdateChromaticAberration(1.0f);
        }
    }
    private void resetAnimation() {
        foreach (AnimatorControllerParameter parameter in anim.parameters) {
            anim.SetBool(parameter.name, false);
        }
    }
private void SetIdle(){
    if (selectedEnemy){
        anim.SetBool("isSelectedEnemy", true);
    }
    anim.SetBool("isIdle", true);
}
#region Dashing Animation controls
    private void DashStart(){
        canDash = false;
    }
    private void DashEnd(){
        resetAnimation();
        SetIdle();
        StartCoroutine("DashCooldown");
    }

    private void invincibilityStart(){
        canTakeDamge = false;
    }
    private void invincibilityEnd(){
        canTakeDamge = true;
    }
    IEnumerator DashCooldown() {
        yield return new WaitForSeconds(1.5f);
        canDash = true;
        yield return 0;
    }
#endregion
#region Blocking Animation Controls
    public void BlockStart(){
        resetAnimation();
        anim.SetBool("blockSafe", true); 
    }

    public void BlockEnd(){
        anim.SetBool("blockSafe", false);
        anim.SetBool("blockEnd", true); 
        SetIdle();
    }
#endregion
#region Punch Animation Controls
    public void startPunch() {
        isPunching = true;
        anim.SetBool("inPunch", true);
    }
    public void endPunch() {
        isPunching = false;
        //resetAnimation();
        anim.SetBool("punch1", false);
        anim.SetBool("punch2", false);
        anim.SetBool("punch3", false);
        anim.SetBool("punch4", false);
        anim.SetBool("punch5", false);
        anim.SetBool("isPunching", false);
        anim.SetBool("attackCooldown", true);
    }
    #endregion
#region Spell Animation Controls
public void drinkEnd(){
    activatedSpell = true;
    anim.SetBool("isDrinking", false);
    if (spellController.currentlySelected() == 0){
        StartCoroutine("SpellCoolDownSlot1");
    }
                
    if (spellController.currentlySelected() == 1){
         StartCoroutine("SpellCoolDownSlot2");
    }

    if (spellController.currentlySelected() == 2){
        StartCoroutine("SpellCoolDownSlot3");
    }
    resetAnimation();
    SetIdle();
}
public void castEnd() {
    resetAnimation();
    SetIdle();
}
public void castSpell() {
    string spellName = "VFX/Spells/Spell_" + spellController.activeSpell();
    GameObject spellToShoot = Resources.Load<GameObject>(spellName);
    GameObject spellPrefab = Instantiate(spellToShoot, rightHand.transform.position, transform.rotation) as GameObject;
}
IEnumerator SpellCoolDownSlot1() {
        yield return new WaitForSeconds(15f);
        canCast = false;
        activatedSpell = false;
        StartCoroutine("CastingCooldown");
        yield return 0;
}
IEnumerator SpellCoolDownSlot2() {
        yield return new WaitForSeconds(15f);
        canCast = false;
        activatedSpell = false;
        StartCoroutine("CastingCooldown");
        yield return 0;
}
IEnumerator SpellCoolDownSlot3() {
        yield return new WaitForSeconds(15f);
        canCast = false;
        activatedSpell = false;
        StartCoroutine("CastingCooldown");
        yield return 0;
}
IEnumerator CastingCooldown() {
        yield return new WaitForSeconds(60f);
        canCast = true;
        yield return 0;
}
#endregion
#region Attack To Idle Animation Controls
    public void endAttackIdle() {
        resetAnimation();
    }
    #endregion
#region Damage Animation Controls
    public void turnOffDamage() {
        resetAnimation();
        SetIdle();
    }
    public void damageStart() {
        resetAnimation();
        anim.SetBool("inDamage", true);
        anim.SetBool("tookDamage", true);
    }
    #endregion
}