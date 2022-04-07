using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(PlayerInformation))]
public class PlayerAttackSystem : MonoBehaviour
{
    private CharacterProperties characterProperties;
    private PlayerInformation playerInformation;

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
        playerInformation = gameObject.GetComponent<PlayerInformation>();
    }

    private void Update()
    {
        Input_AttackingController();
    }

    void Input_AttackingController()
    {
        if (playerInformation.onAttacking == false)
        {
            if (Input.GetButtonDown("Fire1") == true)
            {
                Do_MeleeAttacking();
            }

            if (Input.GetButtonDown("Fire2") == true)
            {
                Do_RangeAttacking();
            }

            if(Input.GetKeyDown(KeyCode.Space) == true)
            {
                Do_MobilityMove();
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

        Vector3 positionToPointTo = gameObject.transform.position + (prepareDirection * characterProperties.CharacterInformation.movementSpeed);
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
    }

    void Do_RangeAttacking()
    {
        playerInformation.onAttacking = true;
        Prepare_BeforeAttacking();
        playerInformation.characterAnimator.SetTrigger("ActiveRangeAttack");

        playerInformation.currentUseSkill = playerInformation.rangeSkill;
    }

    void Do_MobilityMove()
    {
        playerInformation.onAttacking = true;
        Prepare_BeforeAttacking();
        playerInformation.characterAnimator.SetTrigger("ActiveMobility");

        playerInformation.currentUseSkill = playerInformation.mobilitySkill;
    }
}
