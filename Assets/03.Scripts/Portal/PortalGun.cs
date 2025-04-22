using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    public GameObject laser;
    public GameObject[] portals;
    
    private float fireRate = 0.5f;
    private float currentFireRate = 0f;
    private int portalIndex = 0;
    private Ray ray;
    private void Start()
    {
        
    }

    void Update()
    {
        currentFireRate += Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0) && currentFireRate > fireRate)
        {
            laser.SetActive(true);
            portalIndex = 0;
            portals[portalIndex].SetActive(false);
            Fire(portalIndex);
            currentFireRate = 0f;
        }
        
        if (Input.GetKey(KeyCode.Mouse1) && currentFireRate > fireRate)
        {
            laser.SetActive(true);
            portalIndex = 1;
            portals[portalIndex].SetActive(false);
            Fire(portalIndex);
            currentFireRate = 0f;
        }
        
    }

    private void Fire(int portalIndex)
    {
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Portal")) return;
            portals[portalIndex].SetActive(true);
            Debug.Log("Hit Object: " + hit.collider.gameObject.name);
            Vector3 spawnPosition = hit.point;
            Quaternion spawnRotation = Quaternion.LookRotation(hit.normal);
            portals[portalIndex].transform.position = spawnPosition;
            portals[portalIndex].transform.rotation = spawnRotation;
            
        }
        laser.SetActive(false);
    }
}
