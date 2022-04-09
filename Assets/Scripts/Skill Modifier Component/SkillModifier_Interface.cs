using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SkillModifier_Interface
{
    public void Setup_SkillModifierProperties(SkillModifier_BaseProperties modProperties, int characterDamage, GameObject owner, GameObject applier);
    public void Reapply_SkillModifierProperties(SkillModifier_BaseProperties modProperties, int characterDamage, GameObject owner, GameObject applier);
}
