using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Enemy_Thug : MonoBehaviour{
    Animator anim;
    Script_Enemy_Base aiBase;
    GameObject rightHand;
    GameObject leftHand;
    GameObject rightFoot;
    public GameObject punchObj;
    public GameObject kickObj;
    public GameObject rig;
    public GameObject minimapObject;

    public GameObject arrow;

    public float health = 5f;
    float visibilityDistance = 7f;
    float visibilityDistance_multiplyer = 2f;
    bool seenPlayer = false;
    bool isBlocking = false;
    bool blockDecision = false;
    bool midPunch = false;
    bool isDodging = false;

    void Start(){
        anim = GetComponent<Animator>();
        aiBase = GetComponent<Script_Enemy_Base>();
        rightHand = gameObject.transform.Find(
            "Thug_rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand"
            ).gameObject; //get reference to right hand
        leftHand = gameObject.transform.Find(
            "Thug_rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand"
            ).gameObject; //get reference to left hand
        rightFoot = gameObject.transform.Find(
            "Thug_rig/mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot"
            ).gameObject; //get reference to right hand
    }

    void Update(){
        if (!anim.GetBool("isDying")) {
            /*
             * Checks if player is in line of sight OR is being loud around the enemy
             * @returns seenPlayer
             */
            if ((Vector3.Distance(aiBase.getPlayerLocation(), gameObject.transform.position) <= visibilityDistance && aiBase.canSeePlayer()) || 
                (Vector3.Distance(aiBase.getPlayerLocation(), gameObject.transform.position) <= visibilityDistance && (aiBase.isPlayerWalking() || aiBase.isPlayerRunning()))) {
                seenPlayer = true;
                anim.SetBool("isAttackingPlayer", true);
            }
            if (!isDodging) {
                /*
                 * If player is farther than the thug can punch, and is not currently attacking
                 * or blocking, move towards player and start walking animation
                 */
                if (seenPlayer && Vector3.Distance(aiBase.getPlayerLocation(), gameObject.transform.position) > 1.6f &&
                    !anim.GetBool("isAttacking")) {
                    resetAnimation();
                    anim.SetBool("isWalking", true);
                }

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")) {
                    aiBase.move();
                }

                /*
                 * If thug is not currently moving and is not currently attacking
                 * or blocking, make sure its default animation is idle
                 */
                if (aiBase.getAgentVelocity() == new Vector3(0f, 0f, 0f) && seenPlayer && Vector3.Distance(aiBase.getPlayerLocation(), gameObject.transform.position) <= 1.6f &&
                    !anim.GetBool("isAttacking")) {
                    resetAnimation();
                    anim.SetBool("isIdle", true);
                }

                /*
                 * If player is within punching range and is in front of the thug, start attacking
                 */
                if (Vector3.Distance(aiBase.getPlayerLocation(), gameObject.transform.position) <= 1.6f &&
                    aiBase.canSeePlayer() && !anim.GetBool("isAttacking")) {
                    int attackChooser = Random.Range(1, 10);
                    if (attackChooser <= 4) {
                        anim.SetBool("isPunching", true);
                    }
                    if (attackChooser >= 5 && attackChooser <= 8) {
                        anim.SetBool("isChargedPunch", true);
                    }
                    if (attackChooser >= 9 && attackChooser <= 10) {
                        anim.SetBool("isKicking", true);
                    }
                    anim.SetBool("isAttacking", true);
                    isBlocking = false;
                }

                /*
                 * If player goes outside of sight range, rotate towards player
                 */
                if (!aiBase.canSeePlayer() && !anim.GetBool("isAttacking")) {
                    Vector3 targetDirection = aiBase.getPlayerLocation() - transform.position;
                    float rotationSpeed = 3;
                    float singleStep = rotationSpeed * Time.deltaTime;

                    Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

                    transform.rotation = Quaternion.LookRotation(newDirection);
                }

                if (GameObject.Find("PlayerPunch(Clone)") &&
                    !anim.GetBool("tookDamage_light") &&
                    !anim.GetBool("tookDamage_heavy") &&
                    !blockDecision &&
                    seenPlayer && Vector3.Distance(aiBase.getPlayerLocation(), gameObject.transform.position) < 3f) {
                    //if the enemy is in mid punch, don't block
                    if (midPunch) {
                        //don't block
                    } else {
                        blockDecision = true;
                        int blockChooser = Random.Range(1, 4);
                       // Debug.Log("Blocking with: " + blockChooser);
                        if (aiBase.canSeePlayer()) {
                            resetAnimation();
                            if (blockChooser <= 2) {
                                anim.SetBool("isBlocking", true);
                                anim.SetBool("isDodging", false);
                            } else {
                                anim.SetBool("isBlocking", false);
                                anim.SetBool("isDodging", true);
                            }
                        }
                    }
                }
            }
            if (isDodging) {
                transform.Translate((transform.forward * -1) * 3f * Time.deltaTime);
            }

            if (aiBase.getPlayerFocus() == null &&
             Vector3.Distance(aiBase.getPlayerLocation(), gameObject.transform.position) <= 5f){
                 aiBase.playerReferece().GetComponent<Script_Player_Movement>().setFocusEnemy(gameObject);
            }
        }
    }

    //Called via animation event from Anim_Player_Punch
    public void punchRight() {
        GameObject playerHit = Instantiate(punchObj, rightHand.transform.position, rightHand.transform.rotation);
    }
    public void punchLeft() {
        GameObject playerHit = Instantiate(punchObj, leftHand.transform.position, leftHand.transform.rotation);
    }

    public void kick() {
        GameObject playerHit = Instantiate(kickObj, rightFoot.transform.position, rightFoot.transform.rotation);
    }

    public void RecieveDamage(float damage) {
        if (!isBlocking && !anim.GetBool("isDying")) {
            health -= damage;
            if (health > 0) {
                resetAnimation();
                if (damage > 2f) {
                    anim.SetBool("tookDamage_heavy", true);
                    Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.3f);
                } else {
                    anim.SetBool("tookDamage_light", true);
                    Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.1f);
                }
            } else {
                if (damage > 2f) {
                    anim.SetBool("isDying_heavy", true);
                } else {
                    anim.SetBool("isDying_light", true);
                }
                Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.5f);
                anim.SetBool("isDying", true);
                minimapObject.SetActive(false);
                aiBase.playerReferece().GetComponent<Script_Player_Movement>().stopFocus();
                RemoveFromTeam();
            }
        }
    }
    
    private void RemoveFromTeam(){
        //Remove room enemy count
        aiBase.GetSpawnActor().GetComponent<Script_Enemy_Spawning>().RemoveFromList(gameObject);
        for (int i = 0; i < aiBase.getTeam().Count; i++){
            //Remove self from every team list
            aiBase.getTeam()[i].GetComponent<Script_Enemy_Base>().RemoveFromTeam(gameObject);
        }
    }

    private void resetAnimation() {
        foreach (AnimatorControllerParameter parameter in anim.parameters) {
            anim.SetBool(parameter.name, false);
        }
        anim.SetBool("isAttackingPlayer", true);
    }

    #region Punching Animation Controls
    public void inPunch() {
        midPunch = true;
    }
    public void outPunch() {
        midPunch = false;
    }
    public void stopPunch() {
        anim.SetBool("isPunching", false);
    }
    #endregion
    #region Kicking Animation Controls
    public void stopKick() {
        anim.SetBool("isKicking", false);
    }
    #endregion
    #region Blocking Animation Controls
    public void blockingStart() {
        isBlocking = true;
        blockDecision = false;
    }
    public void blockingEnd() {
        isBlocking = false;
    }
    public void turnOffBlocking() {
        anim.SetBool("isBlocking", false);
    }
    #endregion
    #region Charged Punch Animation Controls
    public void turnOff_ChargedAttack() {
        anim.SetBool("isChargedPunch", false);
    }
    #endregion
    #region Damage Animation Controls
    public void turnOff_takeDamageLight() {
        anim.SetBool("tookDamage_light", false);
        anim.SetBool("isBlocking", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isIdle", true);
    }
    public void turnOff_takeDamageHeavy() {
        anim.SetBool("isAttacking", false);
        anim.SetBool("tookDamage_heavy", false);
        anim.SetBool("isBlocking", false);
        anim.SetBool("isIdle", true);
    }
    #endregion
    #region Death Animation Controls
    public void turnOffDeath() {
        transform.position = new Vector3(transform.position.x, -1.5f, transform.position.z);
        anim.SetBool("isDying_heavy", false);
        anim.SetBool("isDying_light", false);
    }
    #endregion
    #region Dodge Back Animation Controls
    public void turnOffDodgeBack() {
        anim.SetBool("isDodging", false);
        anim.SetBool("isIdle", true);
    }
    #endregion
    #region Dodge Back
    public void DodgeBackStart() {
        isDodging = true;
        blockDecision = false;
        anim.SetBool("isPunching", false);
        anim.SetBool("isWalking", false);
    }
    public void DodgeBackEnd() {
        isDodging = false;
        anim.SetBool("isDodging", false);
    }
    #endregion
    #region Turn off Attack
    public void TurnOffAttack() {
        anim.SetBool("isAttacking", false);
    }
    #endregion
    #region Turn on Attack
    public void TurnOnAttack() {
        anim.SetBool("isAttacking", true);
    }
    #endregion
}
