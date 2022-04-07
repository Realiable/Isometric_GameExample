using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraSystem : MonoBehaviour
{
    public GameObject targetPlayer;

    [Range(0.0f, 100.0f)]
    public float cameraLagMultiplier = 100.0f;

    private Vector3 positionOffset;

    private void Start()
    {
        positionOffset = gameObject.transform.position - targetPlayer.transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 currentPosition = gameObject.transform.position;
        Vector3 nextPosition = targetPlayer.transform.position + positionOffset;

        gameObject.transform.position = Vector3.Lerp(currentPosition, nextPosition, cameraLagMultiplier * Time.deltaTime);
    }
}
