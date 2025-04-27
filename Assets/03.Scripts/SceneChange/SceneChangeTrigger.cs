using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeTrigger : MonoBehaviour {
    
    // Private variables
    private Transform trackedTraveller;
    private Vector3 previousOffsetFromPortal;
    public string str;
    void LateUpdate () {
        HandleTravellers ();
    }
    void HandleTravellers () {
        
        if (trackedTraveller == null) return; 

        Vector3 offsetFromPortal = trackedTraveller.position - transform.position;
        int portalSide = Math.Sign (Vector3.Dot (offsetFromPortal, transform.forward));
        int portalSideOld = Math.Sign (Vector3.Dot (previousOffsetFromPortal, transform.forward));
        if (portalSide != portalSideOld) { // **변경된 부분**
            SceneChanger.Instance.SceneChange(str);
        } 
    }
    void OnTravellerEnterTrigger (Transform player)
    {
        if (trackedTraveller != player) {
            trackedTraveller = player;
            previousOffsetFromPortal = player.transform.position - transform.position;
        }
    }

    void OnTriggerEnter (Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer ("Player")) {
            var player = other.GetComponent<Transform> ();
            if (player) {
                OnTravellerEnterTrigger (player);
            }
        }
    }
}