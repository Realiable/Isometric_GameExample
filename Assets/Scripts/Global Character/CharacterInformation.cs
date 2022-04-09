using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
public class CharacterInformation : MonoBehaviour
{
    [Header("Character Share Component")]
    public Rigidbody physicsSystem;
    public CapsuleCollider capsuleCollider;
    public Animator characterAnimator;
    public GameObject damageableChecker;
    public GameObject characterBlocker;
    public GameObject collision_FrontBox;
    public GameObject collision_CircleAround;
    public GameObject projectileSpawnPoint;

    [Header("Character Skill Information")]
    public Skills_BaseInformation meleeSkill;
    public Skills_BaseInformation rangeSkill;
    public Skills_BaseInformation mobilitySkill;

    [Header("Skill Modifier Information")]
    public List<SkillModifier_BaseProperties> meleeSkillModifier;
    public List<SkillModifier_BaseProperties> rangeSkillModifier;
    public List<SkillModifier_BaseProperties> mobilitySkillModifier;

    private CharacterProperties characterProperties;

    [HideInInspector]
    public bool isAlive = true;
    [HideInInspector]
    public bool isFreezingAction = false;
    [HideInInspector]
    public bool onAttacking = false;
    [HideInInspector]
    public bool onMobility = false;
    [HideInInspector]
    public Vector3 movingDirection = new Vector3(0, 0, 0);
    [HideInInspector]
    public SkillAttackType currentUseSkillType = SkillAttackType.Melee;
    [HideInInspector]
    public Skills_BaseInformation currentUseSkill;

    [HideInInspector]
    public List<GameObject> alreadyDamagedObjectList;

    [HideInInspector]
    public bool onReceiveForNextInput = false;

    int defaultCharacterLayer = 0;

    private void Awake()
    {
        if(collision_FrontBox != null)
        {
            collision_FrontBox.SetActive(false);
        }

        if (collision_CircleAround != null)
        {
            collision_CircleAround.SetActive(false);
        }
    }

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
    }

    public void Push_CharacterForward()
    {
        physicsSystem.velocity = new Vector3(0, physicsSystem.velocity.y, 0);
        physicsSystem.AddForce(gameObject.transform.forward * currentUseSkill.PushForwardForce);
    }

    public void Open_AttackCollision()
    {
        alreadyDamagedObjectList.Clear();

        if (currentUseSkill.collisionType == CollisionType.FrontBox)
        {
            collision_FrontBox.SetActive(true);
            collision_FrontBox.transform.localScale = new Vector3(currentUseSkill.collisionWidth, currentUseSkill.collisionHeight, currentUseSkill.collisionRange);
        }
        else if(currentUseSkill.collisionType == CollisionType.CircleAround)
        {
            collision_CircleAround.SetActive(true);
            collision_CircleAround.transform.localScale = new Vector3(currentUseSkill.collisionWidth, currentUseSkill.collisionHeight, currentUseSkill.collisionRange);
        }
    }

    public void Close_AttackCollision()
    {
        collision_FrontBox.SetActive(false);
        collision_CircleAround.SetActive(false);
    }

    public void Spawn_Projectile()
    {
        if(currentUseSkill.spawnProjectileObject != null)
        {
            GameObject projectileObj = Instantiate(currentUseSkill.spawnProjectileObject);
            projectileObj.transform.position = projectileSpawnPoint.transform.position;
            projectileObj.transform.eulerAngles = gameObject.transform.eulerAngles;

            ProjectileBehavior projectileBehavior = projectileObj.GetComponent<ProjectileBehavior>();

            if(projectileBehavior != null)
            {
                projectileBehavior.ownerCharacter = gameObject;
                projectileBehavior.projectileSpeedMultiplier = currentUseSkill.projectileSpeedMultiplier;
                float projectileDamage = characterProperties.CharacterInformation.AttackPower * currentUseSkill.SkillDamageMultiplier;
                projectileBehavior.projectileDamage = Mathf.RoundToInt(projectileDamage);
                projectileBehavior.knockbackForce = currentUseSkill.KnockbackForce;
                projectileBehavior.knockupForce = currentUseSkill.KnockupForce;

                projectileBehavior.skillModifierList = Get_CurrentSkillModifier();
            }
        }

        onReceiveForNextInput = true;
    }

    public void Apply_SpecialBehavior()
    {
        if(currentUseSkill.specialBehavior == SpecialBehavior.PassThroughCharacters)
        {
            characterBlocker.SetActive(false);
            defaultCharacterLayer = gameObject.layer;
            gameObject.layer = 10;
        }
    }

    public void Deapply_SpecialBehavior()
    {
        if (currentUseSkill.specialBehavior == SpecialBehavior.PassThroughCharacters)
        {
            characterBlocker.SetActive(true);
            gameObject.layer = defaultCharacterLayer;
        }
    }

    public void Active_Invincible()
    {
        characterProperties.isInvincible = true;
    }

    public void Deactive_Invincible()
    {
        characterProperties.isInvincible = false;
    }

    public void StartReceive_HoldingInput()
    {
        onReceiveForNextInput = true;
    }

    public virtual void End_Attacking()
    {
        onAttacking = false;
        onMobility = false;
        Close_AttackCollision();
        Deactive_Invincible();

        onReceiveForNextInput = false;
    }

    public void Deal_DamageToTarget(DamageableCollision damageableCollision)
    {
        float finalDamage = characterProperties.CharacterInformation.AttackPower;
        finalDamage = finalDamage * currentUseSkill.SkillDamageMultiplier;
        int finalDamage_Int = Mathf.RoundToInt(finalDamage);

        CharacterProperties damageCharacterProperties = damageableCollision.characterProperties;
        damageCharacterProperties.Receive_Damage(finalDamage_Int);
        damageCharacterProperties.Receive_KnockbackForce(currentUseSkill.KnockbackForce, gameObject);
        damageCharacterProperties.Receive_KnockupForce(currentUseSkill.KnockupForce, gameObject);

        if (currentUseSkill.spawnVFXWhenHit != null)
        {
            GameObject hitVFX = Instantiate(currentUseSkill.spawnVFXWhenHit);
            hitVFX.transform.position = damageableCollision.characterProperties.gameObject.transform.position + new Vector3(0, collision_FrontBox.transform.position.y, 0);
        }

        //print("\"" + characterProperties.gameObject.name + "\"" + " deal " + finalDamage_Int + " damage to " + "\"" + damageCharacterProperties.gameObject.name + "\"");

        alreadyDamagedObjectList.Add(damageableCollision.characterProperties.gameObject);

        Apply_DealDamageModifier(damageableCollision);
        Apply_OnImpactModifier(damageableCollision.characterProperties.gameObject);
    }

    void Apply_DealDamageModifier(DamageableCollision damageableCollision)
    {
        float finalDamage = characterProperties.CharacterInformation.AttackPower;
        finalDamage = finalDamage * currentUseSkill.SkillDamageMultiplier;
        int finalDamage_Int = Mathf.RoundToInt(finalDamage);

        GameObject damagedCharacter = damageableCollision.characterProperties.gameObject;

        List<SkillModifier_BaseProperties> modifierPropertiesList = Get_CurrentSkillModifier();

        for(int index = 0; index < modifierPropertiesList.Count; index++)
        {
            SkillModifier_BaseProperties currentModifier = modifierPropertiesList[index];

            if (currentModifier.ApplyCondition == ApplyEffectCondition.Target_ReceiveDamage)
            {
                string modifierCompName = System.Enum.GetName(typeof(ApplyEffectType), currentModifier.ApplyEffectOnTarget);
                System.Type ComponentType = System.Type.GetType(modifierCompName);

                if(damagedCharacter.GetComponent(ComponentType) == false)
                {
                    damagedCharacter.AddComponent(ComponentType);
                    (damagedCharacter.GetComponent(ComponentType) as SkillModifier_Interface).Setup_SkillModifierProperties(currentModifier, finalDamage_Int, gameObject, gameObject);
                }
                else
                {
                    (damagedCharacter.GetComponent(ComponentType) as SkillModifier_Interface).Reapply_SkillModifierProperties(currentModifier, finalDamage_Int, gameObject, gameObject);
                }
            }
        }
    }

    public void Apply_OnImpactModifier(GameObject impactObject)
    {
        float finalDamage = characterProperties.CharacterInformation.AttackPower;
        finalDamage = finalDamage * currentUseSkill.SkillDamageMultiplier;
        int finalDamage_Int = Mathf.RoundToInt(finalDamage);

        List<SkillModifier_BaseProperties> modifierPropertiesList = Get_CurrentSkillModifier();

        for (int index = 0; index < modifierPropertiesList.Count; index++)
        {
            SkillModifier_BaseProperties currentModifier = modifierPropertiesList[index];

            if (currentModifier.ApplyCondition == ApplyEffectCondition.Target_OnImpact)
            {
                string modifierCompName = System.Enum.GetName(typeof(ApplyEffectType), currentModifier.ApplyEffectOnTarget);
                System.Type ComponentType = System.Type.GetType(modifierCompName);

                if (impactObject.GetComponent(ComponentType) == false)
                {
                    impactObject.AddComponent(ComponentType);
                    (impactObject.GetComponent(ComponentType) as SkillModifier_Interface).Setup_SkillModifierProperties(currentModifier, finalDamage_Int, gameObject, gameObject);
                }
                else
                {
                    (impactObject.GetComponent(ComponentType) as SkillModifier_Interface).Reapply_SkillModifierProperties(currentModifier, finalDamage_Int, gameObject, gameObject);
                }
            }
        }
    }

    List<SkillModifier_BaseProperties> Get_CurrentSkillModifier()
    {
        if (currentUseSkillType == SkillAttackType.Melee)
        {
            return meleeSkillModifier;
        }
        else if (currentUseSkillType == SkillAttackType.Range)
        {
            return rangeSkillModifier;
        }

        return mobilitySkillModifier;
    }

    public void Freeze_AllActionFor(float freezingTime)
    {
        if(isFreezingAction == false)
        {
            StartCoroutine(Freeze_AllActionFor_Coroutine(freezingTime));
        }
    }

    IEnumerator Freeze_AllActionFor_Coroutine(float freezingTime)
    {
        isFreezingAction = true;
        yield return new WaitForSeconds(freezingTime);
        isFreezingAction = false;
    }

    public bool Can_CharacterDoAction()
    {
        if(isAlive == false)
        {
            return false;
        }

        if (isFreezingAction == true)
        {
            return false;
        }

        return true;
    }

    public bool Can_CharacterMove()
    {
        if (isAlive == false)
        {
            return false;
        }

        if (onAttacking == true)
        {
            return false;
        }

        if (isFreezingAction == true)
        {
            return false;
        }

        return true;
    }

    public bool Can_CharacterAttack()
    {
        if (isAlive == false)
        {
            return false;
        }

        if (onAttacking == true)
        {
            return false;
        }

        if (isFreezingAction == true)
        {
            return false;
        }

        return true;
    }

    public bool Can_CharacterMobility()
    {
        if (isAlive == false)
        {
            return false;
        }

        if (onMobility == true)
        {
            return false;
        }

        if (isFreezingAction == true)
        {
            return false;
        }

        return true;
    }
}
