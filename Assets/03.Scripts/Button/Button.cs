using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Rigidbody rb;
    private float a;
    private void Update()
    {
        Spring();
    }

    private void Spring()
    {
        a = transform.localPosition.z;
        if (a > 0)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        rb.AddForce(Vector3.up * (-a));
    }
}
