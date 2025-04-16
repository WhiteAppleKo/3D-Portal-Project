using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform head;

    private void LateUpdate()
    {
        gameObject.transform.position = head.transform.position;
    }
}
