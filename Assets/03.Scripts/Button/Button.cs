using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Rigidbody rb;
    private float a;
    private void FixedUpdate()
    {
        a = transform.localPosition.z;
        if (a > 0)
        {
            rb.velocity = Vector3.zero;
        }
    }
    
    // private void Spring()
    // {
    //     a = transform.localPosition.z;
    //     if (a > 0)
    //     {
    //         rb.velocity = Vector3.zero;
    //         return;
    //     }
    //     rb.AddForce(Vector3.up * (-a));
    // }
    //
    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log(other.gameObject.name);
    //     if (other.gameObject.name == "Cube")
    //     {
    //         
    //     }
    // }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Cube")
        {
            rb.AddForce(Vector3.up * (-a));
        }
    }
}
