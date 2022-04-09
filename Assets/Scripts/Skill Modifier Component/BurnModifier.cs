using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnModifier : MonoBehaviour, SkillModifier_Interface
{
    private CharacterProperties characterProperties;
    private SkillModifier_BaseProperties skillModProperties;

    int characterSkillDamage = 0;
    GameObject effectOwner = null;
    GameObject effectApplier = null;

    private float effectTimer = 0.0f;
    private float totalEffectTime = 0.0f;

    GameObject attachVFX = null;
    GameObject characterVFXAttachPoint = null;

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
    }

    public void Setup_SkillModifierProperties(SkillModifier_BaseProperties modProperties, int characterDamage, GameObject owner, GameObject applier)
    {
        if(characterProperties == null)
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
        totalEffectTime = 0.0f;
    }

    void Setup_CharacterRootPoint()
    {
        if(characterProperties != null && characterProperties.characterModel != null)
        {
            GameObject[] VFXAttachPointList = GameObject.FindGameObjectsWithTag("VFX Attach Point");

            for(int index = 0; index < VFXAttachPointList.Length; index+=1)
            {
                if(VFXAttachPointList[index].transform.root.gameObject == characterProperties.gameObject)
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
        if(skillModProperties.ApplyModifierAttachVFX != null)
        {
            attachVFX = Instantiate(skillModProperties.ApplyModifierAttachVFX);

            if(characterProperties != null && characterVFXAttachPoint != null)
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

    private void FixedUpdate()
    {
        Modifier_Timer();
        AttachVFX_Adjustment();
    }

    void Modifier_Timer()
    {
        effectTimer += Time.deltaTime;
        totalEffectTime += Time.deltaTime;

        if (effectTimer >= 1.0f)
        {
            effectTimer = 0.0f;
            int totalDamage = Mathf.RoundToInt(characterSkillDamage * skillModProperties.CharacterDamageMultiplier);
            totalDamage += skillModProperties.BaseDamage;

            if(totalDamage == 0)
            {
                totalDamage = 1;
            }

            if (characterProperties != null)
            {
                characterProperties.Receive_Damage(totalDamage);
            }
        }

        if (totalEffectTime >= skillModProperties.ApplyEffectDuration)
        {
            if (attachVFX != null)
            {
                ParticleSystem attachVFXComp = attachVFX.GetComponent<ParticleSystem>();

                if (attachVFXComp != null)
                {
                    attachVFXComp.Stop(true);
                    Destroy(attachVFX, 1);
                }
                else
                {
                    Destroy(attachVFX);
                }
            }

            Destroy(this);
        }
    }

    void AttachVFX_Adjustment()
    {
        if(attachVFX != null)
        {
            attachVFX.transform.eulerAngles = skillModProperties.ApplyModifierAttachVFX.transform.eulerAngles;
        }
    }
}
