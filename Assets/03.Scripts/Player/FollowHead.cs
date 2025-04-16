using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform head;

    private void FixedUpdate()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, head.transform.position, 0.05f);
    }
}
