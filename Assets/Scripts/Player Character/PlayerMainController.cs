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

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
        playerInformation = gameObject.GetComponent<PlayerInformation>();

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
        Input_MovementController();
    }

    private void FixedUpdate()
    {
        if (playerInformation.onAttacking == false)
        {
            Input_MovementBehavior();
        }
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
            Vector3 positionToPointTo = gameObject.transform.position + (playerInformation.movingDirection * characterProperties.CharacterInformation.movementSpeed);
            Vector3 currentPosition = gameObject.transform.position;
            float diffPos_X = currentPosition.x - positionToPointTo.x;
            float diffPos_Z = currentPosition.z - positionToPointTo.z;
            float radianToTarget = Mathf.Atan2(diffPos_Z, diffPos_X);
            float degreeToTarget = (radianToTarget * 180.0f) / Mathf.PI;
            float finalDegree = -degreeToTarget - 90.0f;    // Change Counterclockwise Direction to Clockwise Direction.

            gameObject.transform.eulerAngles = new Vector3( 0, finalDegree, 0 );
        }

        playerInformation.physicsSystem.velocity = playerInformation.movingDirection * characterProperties.CharacterInformation.movementSpeed;
        playerInformation.characterAnimator.SetFloat("CurrentVelocity", playerInformation.physicsSystem.velocity.magnitude);
    }

    private void OnCollisionEnter(Collision hitWith)
    {
        Rigidbody hitWithPhysics = hitWith.gameObject.GetComponent<Rigidbody>();

        if(hitWithPhysics != null)
        {
            print("Collide with : " + hitWith.gameObject.name);
        }
    }
}
