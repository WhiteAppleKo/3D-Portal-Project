using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public GameObject bridgeTile;
    public int distance = 0;
    public Ray ray;
    public RaycastHit hit;
    public List<GameObject> bridgeList;

    public void Awake()
    {
        bridgeList = new List<GameObject>();
    }

    public bool rayCast(Ray ray, out RaycastHit hitInfo, int layerMask)
    {
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity,  layerMask))
        {
            hit = hitInfo; // 클래스 멤버 변수에 저장
            return true;   // 레이캐스트 성공
        }
        return false;      // 레이캐스트 실패
    }
    public void CreateBridges(RaycastHit hitInfo)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer($"Bridge")) return;
        Transform hitTransform = hit.transform;
        distance = Mathf.CeilToInt(Vector3.Distance(transform.position, hit.point));
        Vector3 position = transform.localPosition;
        Vector3 direction = (hit.point - transform.position).normalized;
        for (int i = 0; i < distance; i++)
        {
            position += direction;
            GameObject newBridge = Instantiate(bridgeTile, position, Quaternion.identity, transform);
            newBridge.transform.rotation = transform.rotation;
            bridgeList.Add(newBridge);
        }
    }

    public void ClearBridges()
    {
        foreach (var bridge in bridgeList)
        {
            Destroy(bridge);
        }
        bridgeList.Clear();
    }
}
