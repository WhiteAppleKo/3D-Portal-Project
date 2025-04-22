using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeClone : Bridge
{
    private int layerToIgnore;
    private int layerMask;

    private void Awake()
    {
        layerToIgnore = LayerMask.NameToLayer("Portal");
        layerMask = ~(1 << layerToIgnore);
    }

    void Update()
    {
        CreateBridges();
    }
}
