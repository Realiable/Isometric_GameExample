using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldingAction
{
    None = 0,
    Melee = 1,
    Range = 2,
    Mobility = 3,
}

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(PlayerInformation))]
public class PlayerAttackSystem : MonoBehaviour
{
    private CharacterProperties characterProperties;
    private PlayerInformation playerInformation;

    private HoldingAction playerHoldingAction = HoldingAction.None;

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
        playerInformation = gameObject.GetComponent<PlayerInformation>();

        playerInformation.collision_FrontBox.SetActive(false);
        playerInformation.collision_CircleAround.SetActive(false);
    }

    private void Update()
    {
        if(Time.timeScale != 0.0f)
        {
            Input_AttackingController();
        }
    }

    void Input_AttackingController()
    {
        if (playerInformation.Can_CharacterAttack() == true)
        {
            if(playerHoldingAction == HoldingAction.Melee)
            {
                Do_MeleeAttacking();
            }
            else if(playerHoldingAction == HoldingAction.Range)
            {
                Do_RangeAttacking();
            }

            if (Input.GetButtonDown("Fire1") == true)
            {
                Do_MeleeAttacking();
            }

            if (Input.GetButtonDown("Fire2") == true)
            {
                Do_RangeAttacking();
            }
        }
        
        if (playerInformation.Can_CharacterMobility() == true)
        {
            if (playerHoldingAction == HoldingAction.Mobility)
            {
                Do_MobilityMove();
            }

            if (Input.GetKeyDown(KeyCode.Space) == true)
            {
                Do_MobilityMove();
            }
        }

        
        if(playerInformation.onReceiveForNextInput == true)
        {
            if (Input.GetKeyDown(KeyCode.Space) == true)
            {
                playerHoldingAction = HoldingAction.Mobility;
            }
            else if (Input.GetButtonDown("Fire1") == true)
            {
                playerHoldingAction = HoldingAction.Melee;
            }
            else if (Input.GetButtonDown("Fire2") == true)
            {
                playerHoldingAction = HoldingAction.Range;
            }
        }
    }

    void Prepare_BeforeAttacking()
    {
        playerInformation.physicsSystem.velocity = Vector3.zero;
        Vector3 prepareDirection = playerInformation.movingDirection;

        if (prepareDirection == new Vector3(0,0,0))
        {
            prepareDirection = gameObject.transform.forward;
        }

        Vector3 positionToPointTo = gameObject.transform.position + (prepareDirection * characterProperties.CharacterInformation.MovementSpeed);
        Vector3 currentPosition = gameObject.transform.position;
        float diffPos_X = currentPosition.x - positionToPointTo.x;
        float diffPos_Z = currentPosition.z - positionToPointTo.z;
        float radianToTarget = Mathf.Atan2(diffPos_Z, diffPos_X);
        float degreeToTarget = (radianToTarget * 180.0f) / Mathf.PI;
        float finalDegree = -degreeToTarget - 90.0f;    // Change Counterclockwise Direction to Clockwise Direction.

        gameObject.transform.eulerAngles = new Vector3(0, finalDegree, 0);
    }

    void Do_MeleeAttacking()
    {
        playerInformation.onAttacking = true;
        Prepare_BeforeAttacking();
        playerInformation.characterAnimator.SetTrigger("ActiveMeleeAttack");
        playerInformation.currentUseSkill = playerInformation.meleeSkill;
        playerInformation.currentUseSkillType = SkillAttackType.Melee;
        playerInformation.characterAnimator.speed = characterProperties.CharacterInformation.AttackSpeed * playerInformation.currentUseSkill.SkillSpeedMultiplier;

        if (playerInformation.currentUseSkill.applyAimAssist == true)
        {
            Aim_Assist();
        }

        playerHoldingAction = HoldingAction.None;
    }

    void Do_RangeAttacking()
    {
        playerInformation.onAttacking = true;
        Prepare_BeforeAttacking();
        playerInformation.characterAnimator.SetTrigger("ActiveRangeAttack");
        playerInformation.currentUseSkill = playerInformation.rangeSkill;
        playerInformation.currentUseSkillType = SkillAttackType.Range;
        playerInformation.characterAnimator.speed = characterProperties.CharacterInformation.AttackSpeed * playerInformation.currentUseSkill.SkillSpeedMultiplier;

        if (playerInformation.currentUseSkill.applyAimAssist == true)
        {
            Aim_Assist();
        }

        playerHoldingAction = HoldingAction.None;
    }

    void Do_MobilityMove()
    {
        if(playerInformation.onAttacking == true)
        {
            playerInformation.End_Attacking();
        }

        playerInformation.onAttacking = true;
        playerInformation.onMobility = true;
        Prepare_BeforeAttacking();
        playerInformation.characterAnimator.SetTrigger("ActiveMobility");
        playerInformation.currentUseSkill = playerInformation.mobilitySkill;
        playerInformation.currentUseSkillType = SkillAttackType.Mobility;
        playerInformation.characterAnimator.speed = characterProperties.CharacterInformation.AttackSpeed * playerInformation.currentUseSkill.SkillSpeedMultiplier;

        if (playerInformation.currentUseSkill.applyAimAssist == true)
        {
            Aim_Assist();
        }

        playerHoldingAction = HoldingAction.None;
    }

    void Aim_Assist()
    {
        List<GameObject> enemiesInAssistRange = new List<GameObject>();
        float rayCastSwipeRange = 5.0f;
        float rayCastSwipeFrequency = 10.0f;

        Vector3 currentPosition = gameObject.transform.position;
        Vector3 startRayCastSwipePosition = currentPosition - (gameObject.transform.right * (rayCastSwipeRange / 2.0f)) + new Vector3(0, 1, 0);
        Vector3 distancePerSwipe = gameObject.transform.right * (rayCastSwipeRange / rayCastSwipeFrequency);
        float rayCastDistance = playerInformation.currentUseSkill.aimAssistRange;

        for (int index = 0; index < rayCastSwipeFrequency; index++)
        {
            RaycastHit rayHitInfo;
            int layerMask = 1 << 6;

            Vector3 currentRayCastPosition = startRayCastSwipePosition + (distancePerSwipe * index);

            if (Physics.Raycast(currentRayCastPosition, gameObject.transform.forward, out rayHitInfo, rayCastDistance, layerMask) == true)
            {
                if (enemiesInAssistRange.Contains(rayHitInfo.collider.gameObject) == false)
                {
                    enemiesInAssistRange.Add(rayHitInfo.collider.gameObject);
                }
            }
        }

        GameObject nearestTarget = null;
        float nearestDistance = 0.0f;

        for (int index = 0; index < enemiesInAssistRange.Count; index++)
        {
            Vector3 targetPosition = enemiesInAssistRange[index].transform.position;
            float diffPos_X = targetPosition.x - currentPosition.x;
            float diffPos_Z = targetPosition.z - currentPosition.z;
            float radianToTarget = Mathf.Atan2(diffPos_Z, diffPos_X);
            float degreeToTarget = radianToTarget * 180.0f / Mathf.PI;
            float ccwDegreeToTarget = -degreeToTarget + 90.0f;
            float degreeFromPlayer = ccwDegreeToTarget - Get_ConvertAngle(gameObject.transform.eulerAngles.y);
            float convertAngle = Get_ConvertAngle(degreeFromPlayer);
            float convertAngle180 = Get_ConvertAngle180(convertAngle);

            float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);
            float degreeMultiplier = Mathf.Sqrt( convertAngle180 );
            float calculatedDistance = distanceToTarget * degreeMultiplier;

            if (index == 0)
            {
                nearestTarget = enemiesInAssistRange[index];
                nearestDistance = calculatedDistance;
            }
            else
            {
                if(nearestDistance > calculatedDistance)
                {
                    nearestTarget = enemiesInAssistRange[index];
                    nearestDistance = calculatedDistance;
                }
            }
        }

        if(nearestTarget != null)
        {
            Vector3 positionToPointTo = nearestTarget.transform.position;
            float diffPos_X = currentPosition.x - positionToPointTo.x;
            float diffPos_Z = currentPosition.z - positionToPointTo.z;
            float radianToTarget = Mathf.Atan2(diffPos_Z, diffPos_X);
            float degreeToTarget = (radianToTarget * 180.0f) / Mathf.PI;
            float finalDegree = -degreeToTarget - 90.0f;    // Change Counterclockwise Direction to Clockwise Direction.

            gameObject.transform.eulerAngles = new Vector3(0, finalDegree, 0);
        }
    }

    float Get_ConvertAngle(float targetAngle)
    {
        float calculatedDegree = targetAngle;

        if (targetAngle >= 360.0f)
        {
            calculatedDegree = targetAngle % 360.0f;
        }
        else if(targetAngle < 0)
        {
            calculatedDegree = 360 - ( Mathf.Abs(targetAngle) % 360);
        }

        return calculatedDegree;
    }

    float Get_ConvertAngle180(float targetAngle)
    {
        float resultAngle = targetAngle;

        if(resultAngle > 180.0f)
        {
            resultAngle = 360 - resultAngle;
        }

        return resultAngle;
    }
}
