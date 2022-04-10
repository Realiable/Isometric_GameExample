using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(PlayerInformation))]
public class PlayerMainController : MonoBehaviour
{
    private CharacterProperties characterProperties;
    private PlayerInformation playerInformation;
    private AnimatorOverrideController characterAnimatorOverride;

    private int lastedHealth = 100;

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
        playerInformation = gameObject.GetComponent<PlayerInformation>();

        lastedHealth = characterProperties.Get_CurrentHealth();

        Setup_PlayerAttackingMotion();
    }

    void Setup_PlayerAttackingMotion()
    {
        playerInformation.characterAnimator.runtimeAnimatorController = playerInformation.basedCharacterAnimatorController;

        characterAnimatorOverride = new AnimatorOverrideController(playerInformation.characterAnimator.runtimeAnimatorController);
        characterAnimatorOverride.name = "AnimatorOverride Instance";
        playerInformation.characterAnimator.runtimeAnimatorController = characterAnimatorOverride;

        characterAnimatorOverride["Player_MeleeAttack"] = playerInformation.meleeSkill.SkillAnimation;
        characterAnimatorOverride["Player_RangeAttack"] = playerInformation.rangeSkill.SkillAnimation;
        characterAnimatorOverride["Player_Mobility"] = playerInformation.mobilitySkill.SkillAnimation;
    }

    private void Update()
    {
        if(playerInformation.isAlive == true)
        {
            State_HealthCheck();
            State_KnockbackAndKnockupCheck();
            Input_MovementController();
        }
    }

    private void FixedUpdate()
    {
        if (playerInformation.Can_CharacterMove() == true)
        {
            Input_MovementBehavior();
        }

        Vector3 finalVelocity = playerInformation.movingDirection * characterProperties.CharacterInformation.MovementSpeed;
        Vector3 finalVelocityWithGravity = new Vector3(finalVelocity.x, playerInformation.physicsSystem.velocity.y, finalVelocity.z);
        playerInformation.characterAnimator.SetFloat("CurrentVelocity", finalVelocity.magnitude);
    }

    void Input_MovementController()
    {
        Vector3 velocityDirection = new Vector3(0,0,0);

        Vector3 cameraForward = playerInformation.cameraObject.transform.forward;
        Vector3 cameraRight = playerInformation.cameraObject.transform.right;
        Vector3 cameraForwardNoY = new Vector3(cameraForward.x, 0, cameraForward.z);
        Vector3 cameraRightNoY = new Vector3(cameraRight.x, 0, cameraRight.z);
        cameraForwardNoY = cameraForwardNoY.normalized;
        cameraRightNoY = cameraRightNoY.normalized;

        int onForwardBackward = 0;
        int onLeftRight = 0;

        if (Input.GetKey(KeyCode.W) == true)
        {
            velocityDirection += cameraForwardNoY;
            onForwardBackward += 1;
        }

        if (Input.GetKey(KeyCode.S) == true)
        {
            velocityDirection += -cameraForwardNoY;
            onForwardBackward -= 1;
        }

        if (Input.GetKey(KeyCode.D) == true)
        {
            velocityDirection += cameraRightNoY;
            onLeftRight += 1;
        }

        if (Input.GetKey(KeyCode.A) == true)
        {
            velocityDirection += -cameraRightNoY;
            onLeftRight -= 1;
        }

        // Diagonal Calculation.

        if (onForwardBackward != 0 && onLeftRight != 0)
        {
            float radianDegree45 = 45 * Mathf.PI / 180.0f;
            velocityDirection = new Vector3(velocityDirection.x * Mathf.Cos(radianDegree45), 0, velocityDirection.z * Mathf.Sin(radianDegree45));
        }

        playerInformation.movingDirection = velocityDirection;
    }

    void Input_MovementBehavior()
    {
        if(playerInformation.movingDirection != new Vector3(0, 0, 0))
        {
            Vector3 positionToPointTo = gameObject.transform.position + (playerInformation.movingDirection * characterProperties.CharacterInformation.MovementSpeed);
            Vector3 currentPosition = gameObject.transform.position;
            float diffPos_X = currentPosition.x - positionToPointTo.x;
            float diffPos_Z = currentPosition.z - positionToPointTo.z;
            float radianToTarget = Mathf.Atan2(diffPos_Z, diffPos_X);
            float degreeToTarget = (radianToTarget * 180.0f) / Mathf.PI;
            float finalDegree = -degreeToTarget - 90.0f;    // Change Counterclockwise Direction to Clockwise Direction.

            gameObject.transform.eulerAngles = new Vector3( 0, finalDegree, 0 );
        }

        Vector3 finalVelocity = playerInformation.movingDirection * characterProperties.CharacterInformation.MovementSpeed;
        Vector3 finalVelocityWithGravity = new Vector3(finalVelocity.x, playerInformation.physicsSystem.velocity.y, finalVelocity.z);
        playerInformation.physicsSystem.velocity = finalVelocityWithGravity;
    }

    void State_HealthCheck()
    {
        if (characterProperties.Get_CurrentHealth() <= 0)
        {
            playerInformation.isAlive = false;
            playerInformation.damageableChecker.SetActive(false);
            playerInformation.characterBlocker.SetActive(false);
            playerInformation.capsuleCollider.enabled = false;
            playerInformation.physicsSystem.useGravity = false;
            playerInformation.physicsSystem.drag = 2.0f;

            playerInformation.characterAnimator.SetTrigger("ActiveDeath");
            StartCoroutine(PlayerDeath_Sequence());
        }
        else if (lastedHealth != characterProperties.Get_CurrentHealth())
        {
            if (lastedHealth < characterProperties.Get_CurrentHealth())          // Heal
            {

            }
            else if (lastedHealth > characterProperties.Get_CurrentHealth())     // Damage
            {
                float damageReceive = lastedHealth - characterProperties.Get_CurrentHealth();
                float ratioDamageToMaxHealth = (damageReceive / characterProperties.CharacterInformation.CharacterHealthPoint) * 100.0f;

                if (ratioDamageToMaxHealth >= 10.0f)
                {
                    playerInformation.characterAnimator.SetTrigger("ActiveHit");
                    playerInformation.Freeze_AllActionFor(0.5f);
                    playerInformation.End_Attacking();
                }

                print(gameObject.name + " receive : " + damageReceive + " Damage.");
            }

            lastedHealth = characterProperties.Get_CurrentHealth();
        }
    }

    IEnumerator PlayerDeath_Sequence()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        GameplayStatistic.Instance.GameOver_ByPlayerDeath();
    }

    void State_KnockbackAndKnockupCheck()
    {
        if (characterProperties.storedKnockbackForce != Vector3.zero)
        {
            playerInformation.physicsSystem.AddForce(characterProperties.storedKnockbackForce);
            characterProperties.storedKnockbackForce = Vector3.zero;
        }

        if (characterProperties.storedKnockupForce != Vector3.zero)
        {
            playerInformation.physicsSystem.AddForce(characterProperties.storedKnockupForce);
            characterProperties.storedKnockupForce = Vector3.zero;
        }
    }
}
