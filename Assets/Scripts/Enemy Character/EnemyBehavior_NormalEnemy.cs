using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(EnemyInformation))]
public class EnemyBehavior_NormalEnemy : MonoBehaviour
{
    private CharacterProperties characterProperties;
    private EnemyInformation enemyInformation;

    private void Start()
    {
        characterProperties = gameObject.GetComponent<CharacterProperties>();
        enemyInformation = gameObject.GetComponent<EnemyInformation>();
    }

    private void Update()
    {
        //enemyInformation.physicsSystem.velocity = gameObject.transform.forward * characterProperties.CharacterInformation.movementSpeed;
    }
}