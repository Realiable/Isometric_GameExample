using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeLeechModifier : MonoBehaviour, SkillModifier_Interface
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
        CharacterProperties ownerCharacterProperties = effectOwner.GetComponent<CharacterProperties>();
        int lifeLeechAmount = Mathf.RoundToInt( characterSkillDamage * skillModProperties.CharacterDamageMultiplier );
        ownerCharacterProperties.Receive_Healing(lifeLeechAmount);

        Destroy(this);
    }
}
