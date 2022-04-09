using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePlus_2 : MonoBehaviour, SkillModifier_Interface
{
    private CharacterProperties characterProperties;
    private SkillModifier_BaseProperties skillModProperties;

    int characterSkillDamage = 0;
    GameObject effectOwner = null;
    GameObject effectApplier = null;

    GameObject attachVFX = null;
    GameObject characterVFXAttachPoint = null;

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
    }

    public void Setup_SkillModifierProperties(SkillModifier_BaseProperties modProperties, int characterDamage, GameObject owner, GameObject applier)
    {
        if (characterProperties == null)
        {
            characterProperties = gameObject.GetComponent<CharacterProperties>();
        }

        skillModProperties = modProperties;
        characterSkillDamage = characterDamage;
        effectOwner = owner;
        effectApplier = applier;

        Setup_CharacterRootPoint();
        Setup_ModifierInitialVFX();
        Setup_ModifierAttachVFX();

        Activate_ModifierEffect();
    }

    public void Reapply_SkillModifierProperties(SkillModifier_BaseProperties modProperties, int characterDamage, GameObject owner, GameObject applier)
    {
        if (characterProperties == null)
        {
            characterProperties = gameObject.GetComponent<CharacterProperties>();
        }

        skillModProperties = modProperties;
        characterSkillDamage = characterDamage;
        effectOwner = owner;
        effectApplier = applier;

        Setup_ModifierInitialVFX();
    }

    void Setup_CharacterRootPoint()
    {
        if (characterProperties != null && characterProperties.characterModel != null)
        {
            GameObject[] VFXAttachPointList = GameObject.FindGameObjectsWithTag("VFX Attach Point");

            for (int index = 0; index < VFXAttachPointList.Length; index += 1)
            {
                if (VFXAttachPointList[index].transform.root.gameObject == characterProperties.gameObject)
                {
                    characterVFXAttachPoint = VFXAttachPointList[index];
                    break;
                }
            }
        }
    }

    void Setup_ModifierInitialVFX()
    {
        if (skillModProperties.ApplyModifierInitialVFX != null)
        {
            GameObject initialVFX = Instantiate(skillModProperties.ApplyModifierInitialVFX);

            initialVFX.transform.SetParent(gameObject.transform, false);
            initialVFX.transform.localPosition = Vector3.zero + new Vector3(0, gameObject.transform.localScale.y / 2.0f, 0);

            Destroy(initialVFX, 1);
        }
    }

    void Setup_ModifierAttachVFX()
    {
        if (skillModProperties.ApplyModifierAttachVFX != null)
        {
            attachVFX = Instantiate(skillModProperties.ApplyModifierAttachVFX);

            if (characterProperties != null && characterVFXAttachPoint != null)
            {
                attachVFX.transform.SetParent(characterVFXAttachPoint.transform, false);
                attachVFX.transform.localPosition = Vector3.zero;
                attachVFX.transform.eulerAngles = skillModProperties.ApplyModifierAttachVFX.transform.eulerAngles;
            }
            else
            {
                attachVFX.transform.SetParent(gameObject.transform, false);
                attachVFX.transform.localPosition = Vector3.zero + new Vector3(0, gameObject.transform.localScale.y / 2.0f, 0);
            }
        }
    }

    void Activate_ModifierEffect()
    {
        ProjectileBehavior baseProjectileBehavior = effectApplier.GetComponent<ProjectileBehavior>();

        GameObject projectileInstance_01 = Instantiate(effectApplier);
        projectileInstance_01.transform.position = effectApplier.transform.position;
        projectileInstance_01.transform.eulerAngles += new Vector3( 0, 15, 0 );

        ProjectileBehavior projectileBehavior_01 = projectileInstance_01.GetComponent<ProjectileBehavior>();

        if (projectileBehavior_01 != null)
        {
            projectileBehavior_01.ownerCharacter = effectOwner;
            projectileBehavior_01.projectileSpeedMultiplier = baseProjectileBehavior.projectileSpeedMultiplier;
            float projectileDamage = baseProjectileBehavior.projectileDamage * skillModProperties.CharacterDamageMultiplier;
            projectileBehavior_01.projectileDamage = Mathf.RoundToInt(projectileDamage);
            projectileBehavior_01.knockbackForce = baseProjectileBehavior.knockbackForce;
            projectileBehavior_01.knockupForce = baseProjectileBehavior.knockupForce;
        }

        GameObject projectileInstance_02 = Instantiate(effectApplier);
        projectileInstance_02.transform.position = effectApplier.transform.position;
        projectileInstance_02.transform.eulerAngles += new Vector3(0, -15, 0);

        ProjectileBehavior projectileBehavior_02 = projectileInstance_02.GetComponent<ProjectileBehavior>();

        if (projectileBehavior_02 != null)
        {
            projectileBehavior_02.ownerCharacter = effectOwner;
            projectileBehavior_02.projectileSpeedMultiplier = baseProjectileBehavior.projectileSpeedMultiplier;
            float projectileDamage = baseProjectileBehavior.projectileDamage * skillModProperties.CharacterDamageMultiplier;
            projectileBehavior_02.projectileDamage = Mathf.RoundToInt(projectileDamage);
            projectileBehavior_02.knockbackForce = baseProjectileBehavior.knockbackForce;
            projectileBehavior_02.knockupForce = baseProjectileBehavior.knockupForce;
        }
    }
}
