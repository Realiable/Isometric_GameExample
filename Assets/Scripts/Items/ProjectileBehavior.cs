using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public Rigidbody PhysicsSystem;
    public float projectileSpeed = 5.0f;
    public bool destroyOnHit = true;
    public GameObject spawnVFXWhenDestroy;

    [HideInInspector]
    public List<SkillModifier_BaseProperties> skillModifierList;

    [HideInInspector]
    public GameObject ownerCharacter;
    [HideInInspector]
    public float projectileSpeedMultiplier = 1.0f;
    [HideInInspector]
    public int projectileDamage = 5;
    [HideInInspector]
    public float knockbackForce = 0;
    [HideInInspector]
    public float knockupForce = 0;

    bool onDestroy = false;
    bool onApplyInitializeModifier = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsSystem.velocity = gameObject.transform.forward * projectileSpeed * projectileSpeedMultiplier;

        if(onApplyInitializeModifier == false && ownerCharacter != null)
        {
            onApplyInitializeModifier = true;
            Apply_InitializeModifier();
        }
    }

    void Destroy_Projectile()
    {
        if (onDestroy == false && destroyOnHit == true)
        {
            onDestroy = true;

            if(spawnVFXWhenDestroy != null)
            {
                GameObject spawnDestroyVFX = Instantiate(spawnVFXWhenDestroy);
                spawnDestroyVFX.transform.position = gameObject.transform.position;
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DamageableCollision damageableCollision = other.GetComponent<DamageableCollision>();

        if (damageableCollision != null)
        {
            if (damageableCollision.characterProperties.gameObject != ownerCharacter)
            {
                if (damageableCollision.characterProperties.isInvincible == false)
                {
                    damageableCollision.characterProperties.Receive_Damage(projectileDamage);
                    damageableCollision.characterProperties.Receive_KnockbackForce(knockbackForce, gameObject);
                    damageableCollision.characterProperties.Receive_KnockupForce(knockupForce, gameObject);

                    Apply_DealDamageModifier(damageableCollision);
                    Apply_OnImpactModifier(other.gameObject);

                    Destroy_Projectile();
                }
            }
        }
        else
        {
            if (other.gameObject != ownerCharacter)
            {
                CharacterProperties characterProperties = other.GetComponent<CharacterProperties>();

                if ((characterProperties == null || characterProperties.isInvincible == true ) && other.gameObject.layer != 8)
                {
                    Apply_OnImpactModifier(other.gameObject);
                    Destroy_Projectile();
                }
            }
        }
    }

    void Apply_DealDamageModifier(DamageableCollision damageableCollision)
    {
        GameObject damagedCharacter = damageableCollision.characterProperties.gameObject;

        for (int index = 0; index < skillModifierList.Count; index++)
        {
            SkillModifier_BaseProperties currentModifier = skillModifierList[index];

            if (currentModifier.ApplyCondition == ApplyEffectCondition.Target_ReceiveDamage)
            {
                string modifierCompName = System.Enum.GetName(typeof(ApplyEffectType), currentModifier.ApplyEffectOnTarget);
                System.Type ComponentType = System.Type.GetType(modifierCompName);

                if (damagedCharacter.GetComponent(ComponentType) == false)
                {
                    damagedCharacter.AddComponent(ComponentType);
                    (damagedCharacter.GetComponent(ComponentType) as SkillModifier_Interface).Setup_SkillModifierProperties(currentModifier, projectileDamage, ownerCharacter, gameObject);
                }
                else
                {
                    (damagedCharacter.GetComponent(ComponentType) as SkillModifier_Interface).Reapply_SkillModifierProperties(currentModifier, projectileDamage, ownerCharacter, gameObject);
                }
            }
        }
    }

    void Apply_OnImpactModifier(GameObject impactObject)
    {
        List<SkillModifier_BaseProperties> modifierPropertiesList = skillModifierList;

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
                    (impactObject.GetComponent(ComponentType) as SkillModifier_Interface).Setup_SkillModifierProperties(currentModifier, projectileDamage, ownerCharacter, gameObject);
                }
                else
                {
                    (impactObject.GetComponent(ComponentType) as SkillModifier_Interface).Reapply_SkillModifierProperties(currentModifier, projectileDamage, ownerCharacter, gameObject);
                }
            }
        }
    }

    void Apply_InitializeModifier()
    {
        for (int index = 0; index < skillModifierList.Count; index++)
        {
            SkillModifier_BaseProperties currentModifier = skillModifierList[index];

            if (currentModifier.ApplyCondition == ApplyEffectCondition.Skill_Initialize)
            {
                string modifierCompName = System.Enum.GetName(typeof(ApplyEffectType), currentModifier.ApplyEffectOnTarget);
                System.Type ComponentType = System.Type.GetType(modifierCompName);

                if (gameObject.GetComponent(ComponentType) == false)
                {
                    gameObject.AddComponent(ComponentType);
                    (gameObject.GetComponent(ComponentType) as SkillModifier_Interface).Setup_SkillModifierProperties(currentModifier, projectileDamage, ownerCharacter, gameObject);
                }
                else
                {
                    (gameObject.GetComponent(ComponentType) as SkillModifier_Interface).Reapply_SkillModifierProperties(currentModifier, projectileDamage, ownerCharacter, gameObject);
                }
            }
        }
    }
}
