using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInformation : CharacterInformation
{
    [Header("Player Share Component")]
    public GameObject cameraObject;
    public GameObject WeaponSlot_Right;
    public AnimatorOverrideController basedCharacterAnimatorController;

    [Header("Player User Interface")]
    public GameObject UI_HealthBarValue;
    public TMP_Text UIText_CurrentHealthValue;
    public TMP_Text UIText_MaxHealthValue;

    public bool Check_AvaliableToAddMeleeModifier(SkillModifier_BaseProperties skillModifier)
    {
        if(meleeSkillModifier.Contains(skillModifier) == true)
        {
            return false;
        }

        return true;
    }

    public bool Check_AvaliableToAddRangeModifier(SkillModifier_BaseProperties skillModifier)
    {
        if (rangeSkillModifier.Contains(skillModifier) == true)
        {
            return false;
        }

        return true;
    }

    public bool Check_AvaliableToAddMobilityModifier(SkillModifier_BaseProperties skillModifier)
    {
        if (mobilitySkillModifier.Contains(skillModifier) == true)
        {
            return false;
        }

        return true;
    }

    public List<SkillModifier_BaseProperties> Get_CurrentMeleeSkillModifer()
    {
        return meleeSkillModifier;
    }

    public List<SkillModifier_BaseProperties> Get_CurrentRangeSkillModifer()
    {
        return rangeSkillModifier;
    }

    public List<SkillModifier_BaseProperties> Get_CurrentMobilitySkillModifer()
    {
        return mobilitySkillModifier;
    }

    public void Add_SkillModifierToMeleeSkill(SkillModifier_BaseProperties skillModifier)
    {
        meleeSkillModifier.Add(skillModifier);
    }

    public void Add_SkillModifierToRangeSkill(SkillModifier_BaseProperties skillModifier)
    {
        rangeSkillModifier.Add(skillModifier);
    }

    public void Add_SkillModifierToMobilitySkill(SkillModifier_BaseProperties skillModifier)
    {
        mobilitySkillModifier.Add(skillModifier);
    }
}
