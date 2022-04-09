using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCollision_Checker : MonoBehaviour
{
    public CharacterInformation characterInformation;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Damageable") == true)
        {
            DamageableCollision damageableCollision = other.GetComponent<DamageableCollision>();

            if (damageableCollision.characterProperties.gameObject != characterInformation.gameObject)
            {
                if (characterInformation.alreadyDamagedObjectList.Contains(damageableCollision.characterProperties.gameObject) == false)
                {
                    characterInformation.Deal_DamageToTarget(damageableCollision);
                    //print( "Attack Hit : " + damageableCollision.characterProperties.gameObject.name);
                }
            }
        }
        else
        {
            if(other.gameObject.layer != 9)
            {
                characterInformation.Apply_OnImpactModifier(other.gameObject);
            }
        }
    }
}
