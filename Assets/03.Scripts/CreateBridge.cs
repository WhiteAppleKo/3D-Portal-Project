using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBridge : Bridge
{
    public GameObject bridgeCreatorClone;
    private GameObject clone;

    private void OnEnable()
    {
        PortalGun.OnFire += HandlePortalFire;
        ray = new Ray(transform.position, transform.forward);
        CreateBridgeClone();
    }
    
    private void OnDisable()
    {
        PortalGun.OnFire -= HandlePortalFire;

        if (clone != null)
        {
            Bridge bridgeComponent = clone.GetComponent<Bridge>();
            if (bridgeComponent != null)
            {
                bridgeComponent.ClearBridges(); // Bridge의 ClearBridges 호출
            }
            Destroy(clone);
        }

        ClearBridges();
    }

    private void HandlePortalFire(int portalIndex)
    {
        StartCoroutine(HandleBridgeCreation());
    }
    
    private IEnumerator HandleBridgeCreation()
    {
        ClearBridges();
        if (clone != null)
        {
            clone.GetComponent<Bridge>().ClearBridges();
        }
        clone.GetComponent<Bridge>().ClearBridges();
        Destroy(clone);
        yield return null; // 한 프레임 대기
        CreateBridgeClone();
    }
    public void CreateBridgeClone()
    {
        if (rayCast(ray, out hit, ~0))
        {
            Debug.Log($"Hit Point: {hit.point}, Hit Object: {hit.collider.gameObject.name}");
            CreateBridges(hit);
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Portal"))
            {
                if(clone != null) Destroy(clone);
                
                // 충돌한 포탈
                Transform portalTransform = hit.collider.transform;
                // 현재 포탈의 로컬 좌표를 반대편 포탈의 월드 좌표로 변환
                Vector3 localPosition = portalTransform.InverseTransformPoint(hit.point); // 충돌 지점의 로컬 좌표
                Transform linkedPortalTransform = hit.collider.gameObject.GetComponent<Portal>().linkedPortal.transform;
                Vector3 linkedPortalPosition = linkedPortalTransform.TransformPoint(localPosition);
                
                // 반대편 포탈의 위치에 클론 생성
                clone = Instantiate(bridgeCreatorClone, linkedPortalPosition, Quaternion.identity);
                clone.transform.rotation = linkedPortalTransform.rotation;
                clone.SetActive(true);
            }
        }
    }
}
