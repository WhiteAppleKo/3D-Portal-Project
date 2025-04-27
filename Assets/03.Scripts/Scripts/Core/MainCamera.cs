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
        //Portal.OnEnablePortal += AddPortal;
        //Portal.OnDisablePortal += DelPortal;
    }
    
    private void OnEnable()
    {
        Portal.OnEnablePortal += AddPortal;
        Portal.OnDisablePortal += DelPortal;
    }
    private void AddPortal(Portal portal)
    {
        portals.Add(portal);
        portal.playerCam = Camera.main;
    }
    
    private void DelPortal(Portal portal)
    {
        portals.Remove(portal);
    }

    public void PortalFinder()
    {
        // 현재 씬에서 활성화된 모든 Portal 오브젝트를 찾음
        Portal[] allPortals = FindObjectsOfType<Portal>();
        foreach (var portal in allPortals)
        {
            if (portal.gameObject.activeInHierarchy) // 활성화된 오브젝트만 추가
            {
                AddPortal(portal);
            }
        }
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