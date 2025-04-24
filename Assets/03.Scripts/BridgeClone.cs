using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeClone : Bridge
{
    private int layerToIgnore;
    private int layerMask;

    private new void Awake()
    {
        ray = new Ray(transform.position, transform.forward);
        layerToIgnore = LayerMask.NameToLayer("Portal");
        layerMask = ~(1 << layerToIgnore);
        
        CloneCreateBridge();
    }

    public void CloneCreateBridge()
    {
        ClearBridges();
        if (rayCast(ray, out hit, layerMask)) // layerMask를 전달하여 Portal 레이어 무시
        {
            CreateBridges(hit);
        }
    }
}
