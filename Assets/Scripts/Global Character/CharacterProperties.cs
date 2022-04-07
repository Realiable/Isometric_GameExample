using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperties : MonoBehaviour
{
    public Character_BaseProperties CharacterInformation;
    private int currentHealthPoint = 0;

    private void Awake()
    {
        currentHealthPoint = CharacterInformation.characterHealthPoint;
    }

    public void Receive_Damage(int damageAmount)
    {
        currentHealthPoint -= damageAmount;

        if(currentHealthPoint <= 0)
        {
            currentHealthPoint = 0;
        }
    }

    public void Receive_Healing(int healingAmount)
    {
        currentHealthPoint += healingAmount;

        if (currentHealthPoint > CharacterInformation.characterHealthPoint)
        {
            currentHealthPoint = CharacterInformation.characterHealthPoint;
        }
    }
}
