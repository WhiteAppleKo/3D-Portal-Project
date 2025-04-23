using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float power;
    public void FireLaunch(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction * power;
    }
}
