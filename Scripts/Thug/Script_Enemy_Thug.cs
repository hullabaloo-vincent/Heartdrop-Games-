using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Enemy_Thug : MonoBehaviour
{

    void Start()
    {
        anim = GetComponent<Animator>();
        aiBase = GetComponent<Script_Enemy_Base>();
        rd = GetComponent<Rigidbody>();
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

    void Update()
    {
        if (!anim.GetBool("isDying"))
        {
            /*
             * Checks if player is in line of sight OR is being loud around the enemy
             * @returns seenPlayer
             */
            if ((Vector3.Distance(aiBase.GetPlayerLocation(), gameObject.transform.position) <= visibilityDistance && aiBase.CanSeePlayer()) ||
                (Vector3.Distance(aiBase.GetPlayerLocation(), gameObject.transform.position) <= visibilityDistance && (aiBase.IsPlayerWalking() || aiBase.IsPlayerRunning())))
            {
                seenPlayer = true;
                anim.SetBool("isAttackingPlayer", true);
            }
            if (!isDodging)
            {
                /*
                 * If player is farther than the thug can punch, and is not currently attacking
                 * or blocking, move towards player and start walking animation
                 */
                if (seenPlayer && Vector3.Distance(aiBase.GetPlayerLocation(), gameObject.transform.position) > 1.6f &&
                    !anim.GetBool("isAttacking"))
                {
                    resetAnimation();
                    anim.SetBool("isWalking", true);
                }

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") &&
                !anim.GetBool("isPunching") && !anim.GetBool("isKicking"))
                {
                    aiBase.UnlockMovement();
                    aiBase.move();
                }

                /*
                 * If thug is not currently moving and is not currently attacking
                 * or blocking, make sure its default animation is idle
                 */
                if (aiBase.GetAgentVelocity() == new Vector3(0f, 0f, 0f) && seenPlayer && Vector3.Distance(aiBase.GetPlayerLocation(), gameObject.transform.position) <= 1.6f &&
                    !anim.GetBool("isAttacking"))
                {
                    resetAnimation();
                    anim.SetBool("isIdle", true);
                }

                /*
                * The enemy is walking yet they are moving nowhere; the enemy might be stuck behind
                * other entities. This will change their course to flanking instead
                */
                if (aiBase.GetAgentVelocity() == new Vector3(0f, 0f, 0f) && seenPlayer)
                {
                    aiBase.SetFlankPosition();
                    aiBase.SetFlank(true);
                }

                //Remove flank behavior if all teammates are dead
                if (aiBase.GetTeam().Count == 0)
                {
                    aiBase.SetFlank(false);
                }

                /*
                 * If player is within punching range and is in front of the thug, start attacking
                 */
                if (Vector3.Distance(aiBase.GetPlayerLocation(), gameObject.transform.position) <= 1.5f &&
                    aiBase.CanSeePlayerNarrow() && !anim.GetBool("isAttacking") && !aiBase.IsFlanking() && !_FixRotation)
                {
                    aiBase.stopMoving();
                    int attackChooser = Random.Range(1, 10);
                    if (attackChooser <= 4)
                    {
                        anim.SetBool("isPunching", true);
                    }
                    if (attackChooser >= 5 && attackChooser <= 8)
                    {
                        anim.SetBool("isChargedPunch", true);
                    }
                    if (attackChooser >= 9 && attackChooser <= 10)
                    {
                        anim.SetBool("isKicking", true);
                    }
                    anim.SetBool("isAttacking", true);
                    isBlocking = false;
                }

                /*
                 * If player goes outside of sight range, rotate towards player
                 */
                if (!aiBase.CanSeePlayerNarrow() && !anim.GetBool("isAttacking") && !aiBase.IsFlanking())
                {
                    _FixRotation = true;
                    aiBase.RotateEnemy(aiBase.GetPlayerLocation());
                }
                else if (aiBase.CanSeePlayerNarrow() && _FixRotation)
                {
                    _FixRotation = false;
                }
                //If the enemy is attacking but cannot see the player anymore try to move to a new spot
                if (!aiBase.CanSeePlayer() && !aiBase.IsFlanking() && anim.GetBool("isAttacking"))
                {
                    aiBase.SetFlankPosition();
                    aiBase.SetFlank(true);
                }

                //Turn off flanking if the player is really close (EX: Player walks past enemy moving to flanking position)
                if (Vector3.Distance(aiBase.GetPlayerLocation(), gameObject.transform.position) <= 1.5f && aiBase.IsFlanking())
                {
                    aiBase.SetFlank(false);
                }

                if (GameObject.Find("PlayerPunch(Clone)") &&
                    !anim.GetBool("tookDamage_light") &&
                    !anim.GetBool("tookDamage_heavy") &&
                    !blockDecision &&
                    seenPlayer && Vector3.Distance(aiBase.GetPlayerLocation(), gameObject.transform.position) < 3f)
                {
                    //if the enemy is in mid punch, don't block
                    if (midPunch)
                    {
                        //don't block
                    }
                    else
                    {
                        blockDecision = true;
                        int blockChooser = Random.Range(1, 4);
                        // Debug.Log("Blocking with: " + blockChooser);
                        if (aiBase.CanSeePlayer())
                        {
                            resetAnimation();
                            //turn off physics forces
                            //SetRigidBodyKinematic(true);
                            if (blockChooser <= 2)
                            {
                                anim.SetBool("isBlocking", true);
                                anim.SetBool("isDodging", false);
                            }
                            else
                            {
                                anim.SetBool("isBlocking", false);
                                anim.SetBool("isDodging", true);
                            }
                        }
                    }
                }
            }
            if (isDodging)
            {
                //  transform.Translate((transform.forward * -1) * 3f * Time.deltaTime);
            }

            if (aiBase.GetPlayerFocus() == null &&
             Vector3.Distance(aiBase.GetPlayerLocation(), gameObject.transform.position) <= 5f)
            {
                aiBase.PlayerReferece().GetComponent<Script_Player_Movement>().SetFocusEnemy(gameObject);
                //Tells the team that it is being focused by the player
                aiBase.PlayerFocused();
            }
        }
    }

    //Called via animation event from Anim_Player_Punch
    public void punchRight()
    {
        GameObject playerHit = Instantiate(punchObj, rightHand.transform.position, rightHand.transform.rotation);
    }
    public void punchLeft()
    {
        GameObject playerHit = Instantiate(punchObj, leftHand.transform.position, leftHand.transform.rotation);
    }

    public void kick()
    {
        GameObject playerHit = Instantiate(kickObj, rightFoot.transform.position, rightFoot.transform.rotation);
    }

    public void RecieveDamage(float damage)
    {
        if (!isBlocking && !anim.GetBool("isDying") && CanRecieveDamage)
        {
            health -= damage;
            if (health > 0)
            {
                resetAnimation();
                CanRecieveDamage = false;
                if (damage > 2f)
                {
                    anim.SetBool("tookDamage_heavy", true);
                    Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.3f);
                }
                else
                {
                    anim.SetBool("tookDamage_light", true);
                    Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.1f);
                }
            }
            else
            {
                if (damage > 2f)
                {
                    anim.SetBool("isDying_heavy", true);
                }
                else
                {
                    anim.SetBool("isDying_light", true);
                }
                Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.5f);
                anim.SetBool("isDying", true);
                minimapObject.SetActive(false);
                aiBase.PlayerReferece().GetComponent<Script_Player_Movement>().StopFocus();
                RemoveFromTeam();
            }
            StartCoroutine("DamageBuffer");
        }
    }
    IEnumerator DamageBuffer()
    {
        yield return new WaitForSeconds(0.2f);
        CanRecieveDamage = true;
        yield return 0;
    }

    private void RemoveFromTeam()
    {
        //Remove room enemy count
        aiBase.GetSpawnActor().GetComponent<Script_Enemy_Spawning>().RemoveFromList(gameObject);
        for (int i = 0; i < aiBase.GetTeam().Count; i++)
        {
            //Remove self from every team list
            aiBase.GetTeam()[i].GetComponent<Script_Enemy_Base>().RemoveFromTeam(gameObject);
        }
    }

    private void resetAnimation()
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
        anim.SetBool("isAttackingPlayer", true);
    }

    #region Punching Animation Controls
    public void inPunch()
    {
        midPunch = true;
    }
    public void outPunch()
    {
        midPunch = false;
    }
    public void stopPunch()
    {
        anim.SetBool("isPunching", false);
    }
    #endregion
    #region Kicking Animation Controls
    public void stopKick()
    {
        anim.SetBool("isKicking", false);
    }
    #endregion
    #region Blocking Animation Controls
    public void blockingStart()
    {
        aiBase.stopMoving();
        isBlocking = true;
        blockDecision = false;
    }
    public void blockingEnd()
    {
        isBlocking = false;
    }
    public void turnOffBlocking()
    {
        anim.SetBool("isBlocking", false);
    }
    #endregion
    #region Charged Punch Animation Controls
    public void turnOff_ChargedAttack()
    {
        anim.SetBool("isChargedPunch", false);
    }
    #endregion
    #region Damage Animation Controls
    public void turnOff_takeDamageLight()
    {
        anim.SetBool("tookDamage_light", false);
        anim.SetBool("isBlocking", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isIdle", true);
    }
    public void turnOff_takeDamageHeavy()
    {
        anim.SetBool("isAttacking", false);
        anim.SetBool("tookDamage_heavy", false);
        anim.SetBool("isBlocking", false);
        anim.SetBool("isIdle", true);
    }
    #endregion
    #region Death Animation Controls
    public void turnOffDeath()
    {
        transform.position = new Vector3(transform.position.x, -1.5f, transform.position.z);
        anim.SetBool("isDying_heavy", false);
        anim.SetBool("isDying_light", false);
        rd.isKinematic = true;
        //Destroys the collider on the enemy
        Destroy(GetComponent<CapsuleCollider>());
    }
    #endregion
    #region Dodge Back Animation Controls
    public void turnOffDodgeBack()
    {
        anim.SetBool("isDodging", false);
        anim.SetBool("isIdle", true);
    }
    #endregion
    #region Dodge Back
    public void DodgeBackStart()
    {
        aiBase.stopMoving();
        isDodging = true;
        blockDecision = false;
        anim.SetBool("isPunching", false);
        anim.SetBool("isWalking", false);
    }
    public void DodgeBackEnd()
    {
        isDodging = false;
        anim.SetBool("isDodging", false);
    }
    #endregion
    #region Turn off Attack
    public void TurnOffAttack()
    {
        anim.SetBool("isAttacking", false);
    }
    #endregion
    #region Turn on Attack
    public void TurnOnAttack()
    {
        aiBase.stopMoving();
        anim.SetBool("isAttacking", true);
    }
    #endregion

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

    Rigidbody rd;

    public float health = 5f;
    float visibilityDistance = 7f;
    float visibilityDistance_multiplyer = 2f;
    bool seenPlayer = false;
    bool isBlocking = false;
    bool blockDecision = false;
    bool midPunch = false;
    bool isDodging = false;
    bool CanRecieveDamage = true;
    private bool _FixRotation = false;
}
