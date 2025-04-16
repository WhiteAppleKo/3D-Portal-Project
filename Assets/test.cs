using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject camera;
    void Update()
    {
        transform.position = camera.transform.forward * 20;
    }
}
