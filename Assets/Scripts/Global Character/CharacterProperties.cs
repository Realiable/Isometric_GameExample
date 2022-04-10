using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperties : MonoBehaviour
{
    public Character_BaseProperties CharacterInformation;
    public GameObject characterModel;
    private int currentHealthPoint = 0;

    [HideInInspector]
    public Vector3 storedKnockbackForce = Vector3.zero;
    [HideInInspector]
    public Vector3 storedKnockupForce = Vector3.zero;

    [HideInInspector]
    public bool isInvincible = false;

    private void Awake()
    {
        currentHealthPoint = CharacterInformation.CharacterHealthPoint;
    }

    public void Receive_Damage(int damageAmount)
    {
        if (isInvincible == false)
        {
            currentHealthPoint -= damageAmount;

            if (currentHealthPoint <= 0)
            {
                currentHealthPoint = 0;
            }
        }
    }

    public void Receive_Healing(int healingAmount)
    {
        if (currentHealthPoint > 0)
        {
            currentHealthPoint += healingAmount;

            if (currentHealthPoint > CharacterInformation.CharacterHealthPoint)
            {
                currentHealthPoint = CharacterInformation.CharacterHealthPoint;
            }
        }
    }

    public int Get_CurrentHealth()
    {
        return currentHealthPoint;
    }

    public void Receive_KnockbackForce(float knockbackForce, GameObject knockbackSource)
    {
        Vector3 characterPosition = gameObject.transform.position;
        characterPosition = new Vector3(characterPosition.x, 0, characterPosition.z);
        Vector3 sourcePosition = knockbackSource.transform.position;
        sourcePosition = new Vector3(sourcePosition.x, 0, sourcePosition.z);
        Vector3 directionFromSource = (characterPosition - sourcePosition).normalized;

        storedKnockbackForce += directionFromSource * knockbackForce;
    }

    public void Receive_KnockupForce(float knockupForce, GameObject knockupSource)
    {
        storedKnockupForce = Vector3.up * knockupForce;
    }
}
