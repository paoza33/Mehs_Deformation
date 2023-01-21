using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raindrop_Infinite : MonoBehaviour
{
    Vector3 posInit;
    private void Start()
    {
        posInit = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.position = posInit;
    }
}
