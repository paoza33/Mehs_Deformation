using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFallen : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnMouseDown()
    {
        rb.useGravity = true;
    }
}
