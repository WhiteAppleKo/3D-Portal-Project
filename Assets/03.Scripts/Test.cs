using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : PortalTraveller
{
    public GameObject bridgeTile;
    private Transform startTransform;
    private List<GameObject> bridgeTiles;
    private Vector3 targetPosition;
    private void Start()
    {
        startTransform = gameObject.transform;
        bridgeTiles = new List<GameObject>();
        // 초기 목표 위치 설정 (현재 위치에서 로컬 z축으로 1만큼 이동)
        targetPosition = transform.localPosition + new Vector3(0, 0, 1);
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 5f);
        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
        {
            targetPosition += new Vector3(0, 0, 1); // 로컬 z축으로 1만큼 이동
            CreateBridges();
        }
    }

    public void CreateBridges()
    {
        Vector3 position = transform.position;
        Instantiate(bridgeTile, position, Quaternion.identity);
    }
}
