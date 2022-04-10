using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(EnemyInformation))]
public class EnemyBehavior_NormalEnemy : EnemyBehavior_Base
{
    private CharacterProperties characterProperties;
    private EnemyInformation enemyInformation;
    private int lastedHealth = 100;

    private AI_BehaviorState currentAIState = AI_BehaviorState.Idle;
    private float attackDelayTimer = 0.0f;

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
        enemyInformation = gameObject.GetComponent<EnemyInformation>();

        lastedHealth = characterProperties.Get_CurrentHealth();
    }

    private void FixedUpdate()
    {
        if(enemyInformation.isAlive == true)
        {
            State_HealthCheck();
            State_KnockbackAndKnockupCheck();

            AI_Behavior();
        }
    }

    void State_HealthCheck()
    {
        if(characterProperties.Get_CurrentHealth() <= 0)
        {
            enemyInformation.isAlive = false;
            enemyInformation.damageableChecker.SetActive(false);
            enemyInformation.characterBlocker.SetActive(false);
            enemyInformation.capsuleCollider.enabled = false;
            enemyInformation.physicsSystem.useGravity = false;
            enemyInformation.physicsSystem.drag = 2.0f;

            enemyInformation.characterAnimator.SetTrigger("ActiveDeath");
            StartCoroutine(Death_Sequence());
        }
        else if(lastedHealth != characterProperties.Get_CurrentHealth())
        {
            if(lastedHealth < characterProperties.Get_CurrentHealth())          // Heal
            {
                
            }
            else if(lastedHealth > characterProperties.Get_CurrentHealth())     // Damage
            {
                float damageReceive = lastedHealth - characterProperties.Get_CurrentHealth();
                float ratioDamageToMaxHealth = ( damageReceive / characterProperties.CharacterInformation.CharacterHealthPoint ) * 100.0f;

                if(ratioDamageToMaxHealth >= 10.0f)
                {
                    enemyInformation.characterAnimator.SetTrigger("ActiveHit");
                    enemyInformation.Freeze_AllActionFor(0.5f);
                    enemyInformation.End_Attacking();
                }

                print( gameObject.name + " receive : " + damageReceive + " Damage.");
            }

            lastedHealth = characterProperties.Get_CurrentHealth();
        }
    }

    void State_KnockbackAndKnockupCheck()
    {
        if (characterProperties.storedKnockbackForce != Vector3.zero)
        {
            enemyInformation.physicsSystem.AddForce(characterProperties.storedKnockbackForce);
            characterProperties.storedKnockbackForce = Vector3.zero;
        }

        if (characterProperties.storedKnockupForce != Vector3.zero)
        {
            enemyInformation.physicsSystem.AddForce(characterProperties.storedKnockupForce);
            characterProperties.storedKnockupForce = Vector3.zero;
        }
    }

    void AI_Behavior()
    {
        if(targetToAttack != null)
        {
            Vector3 currentVelocity = enemyInformation.physicsSystem.velocity;
            Vector3 currentVelocityNoY = new Vector3(currentVelocity.x, 0, currentVelocity.z);
            enemyInformation.characterAnimator.SetFloat("CurrentVelocity", currentVelocityNoY.magnitude);

            if (attackDelayTimer > 0)
            {
                attackDelayTimer -= Time.deltaTime;

                if(enemyInformation.enemyAttackDelay - attackDelayTimer >= 0.1f)
                {
                    Rotate_ToPlayer();
                }

                if (attackDelayTimer < 0)
                {
                    attackDelayTimer = 0.0f;
                }
            }
            else
            {
                if (currentAIState == AI_BehaviorState.Idle)
                {
                    Rotate_ToPlayer();
                }
                else if (currentAIState == AI_BehaviorState.MoveToTarget)
                {
                    Rotate_ToPlayer();

                    if (Check_ForAttacking() == false)
                    {
                        Vector3 targetHorizontalVelocity = gameObject.transform.forward * characterProperties.CharacterInformation.MovementSpeed;
                        enemyInformation.physicsSystem.velocity = new Vector3(targetHorizontalVelocity.x, currentVelocity.y, targetHorizontalVelocity.z);
                    }
                }
                else if (currentAIState == AI_BehaviorState.Attacking)
                {
                    if (enemyInformation.onAttacking == false)
                    {
                        currentAIState = AI_BehaviorState.MoveToTarget;
                        attackDelayTimer = enemyInformation.enemyAttackDelay;
                    }
                }
            }
        }
    }

    void Rotate_ToPlayer()
    {
        Vector3 enemyLocation = gameObject.transform.position;
        Vector3 targetLocation = targetToAttack.transform.position;

        float DiffPos_X = targetLocation.x - enemyLocation.x;
        float DiffPos_Z = targetLocation.z - enemyLocation.z;
        float radianToPlayer = Mathf.Atan2(DiffPos_Z, DiffPos_X);
        float degreeToPlayer = radianToPlayer * 180.0f / Mathf.PI;
        float ccwDegreeToTarget = -degreeToPlayer + 90.0f;

        Quaternion currentQuaternion = gameObject.transform.rotation;
        Quaternion targetQuaternion = Quaternion.Euler(0, ccwDegreeToTarget, 0);
        gameObject.transform.rotation = Quaternion.Lerp(currentQuaternion, targetQuaternion, enemyInformation.rotateToTargetSpeed);
    }

    bool Check_ForAttacking()
    {
        Vector3 enemyLocation = gameObject.transform.position;
        enemyLocation = new Vector3(enemyLocation.x, 0, enemyLocation.z);
        Vector3 targetLocation = targetToAttack.transform.position;
        targetLocation = new Vector3(targetLocation.x, 0, targetLocation.z);

        float distanceToTarget = Vector3.Distance(enemyLocation, targetLocation);

        if (enemyInformation.meleeSkill != null)
        {
            float meleeSkillRange = enemyInformation.meleeSkill.collisionRange * 1.1f;

            if(distanceToTarget <= meleeSkillRange)
            {
                enemyInformation.currentUseSkill = enemyInformation.meleeSkill;
                enemyInformation.currentUseSkillType = SkillAttackType.Melee;

                currentAIState = AI_BehaviorState.Attacking;
                enemyInformation.onAttacking = true;
                enemyInformation.characterAnimator.SetTrigger("ActiveMeleeAttack");

                return true;
            }
        }

        if (enemyInformation.rangeSkill != null)
        {
            float rangeSkillRange = enemyInformation.rangeSkill.aimAssistRange * 1.1f;

            if (distanceToTarget <= rangeSkillRange)
            {
                enemyInformation.currentUseSkill = enemyInformation.rangeSkill;
                enemyInformation.currentUseSkillType = SkillAttackType.Range;

                currentAIState = AI_BehaviorState.Attacking;
                enemyInformation.onAttacking = true;
                enemyInformation.characterAnimator.SetTrigger("ActiveRangeAttack");

                return true;
            }
        }

        return false;
    }

    IEnumerator Death_Sequence()
    {
        yield return new WaitForSeconds(0.5f);
        GameplayStatistic.Instance.Add_PlayerKillCount();
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision hitWith)
    {
        if(hitWith.gameObject.layer == 11)
        {
            currentAIState = AI_BehaviorState.MoveToTarget;
        }
    }
}