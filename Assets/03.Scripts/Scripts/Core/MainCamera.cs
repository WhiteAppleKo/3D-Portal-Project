using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MainCamera : MonoBehaviour {

    public List<Portal> portals;

    void Awake () {
        portals = new List<Portal>();
        Portal.OnEnablePortal += AddPortal;
        Portal.OnDisablePortal += DelPortal;
    }

    private void OnEnable()
    {
        Portal.OnEnablePortal += AddPortal;
        Portal.OnDisablePortal += DelPortal;
    }
    private void AddPortal(Portal portal)
    {
        portals.Add(portal);
    }
    
    private void DelPortal(Portal portal)
    {
        portals.Remove(portal);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < portals.Count; i++) {
            portals[i].PrePortalRender ();
        }
        for (int i = 0; i < portals.Count; i++) {
            portals[i].Render ();
        }

        for (int i = 0; i < portals.Count; i++) {
            portals[i].PostPortalRender ();
        }
    }
}