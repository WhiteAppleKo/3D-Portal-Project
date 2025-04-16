using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    public Transform firePos;
    public Transform aimTarget;
    public GameObject laser;
    public GameObject[] portals;
    
    private float fireRate = 0.5f;
    private float currentFireRate = 0f;
    private int portalIndex = 0;

    void Update()
    {
        currentFireRate += Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0) && currentFireRate > fireRate)
        {
            laser.SetActive(true);
            portalIndex = 0;
            Fire(portalIndex);
            currentFireRate = 0f;
        }
        
        if (Input.GetKey(KeyCode.Mouse1) && currentFireRate > fireRate)
        {
            laser.SetActive(true);
            portalIndex = 1;
            Fire(portalIndex);
            currentFireRate = 0f;
        }
        
    }

    private void Fire(int portalIndex)
    {
        Vector3 direction = (aimTarget.position - firePos.position).normalized;  
        Ray ray = new Ray(firePos.position, direction);
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit Object: " + hit.collider.gameObject.name);
            Vector3 spawnPosition = hit.point + hit.normal;
            Quaternion spawnRotation = Quaternion.LookRotation(hit.normal);
            portals[portalIndex].transform.position = spawnPosition;
            portals[portalIndex].transform.rotation = spawnRotation;
            
        }
        laser.SetActive(false);
    }
}
