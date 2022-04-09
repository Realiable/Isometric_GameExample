using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroySelf : MonoBehaviour
{
    public float timeToAutoDestroy = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToAutoDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
