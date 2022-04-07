using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlockerSetup : MonoBehaviour
{
    public Collider blockerCollider;
    public Collider[] ignoreColliders;
    
    void Start()
    {
        for(int index = 0; index < ignoreColliders.Length; index++)
        {
            Physics.IgnoreCollision(blockerCollider, ignoreColliders[index], true);
        }
    }
}
