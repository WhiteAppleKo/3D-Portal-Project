using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public GameObject bridgeTile;
    public float count = 0;
    public Ray ray;
    public RaycastHit hit;

    public bool rayCast(out RaycastHit hitInfo)
    {
        if (Physics.Raycast(ray, out hitInfo))
        {
            hit = hitInfo; // 클래스 멤버 변수에 저장
            return true;   // 레이캐스트 성공
        }
        return false;      // 레이캐스트 실패
    }
    public void CreateBridges()
    {
        if (rayCast(out hit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer($"Bridge")) return;
            Transform hitTransform = hit.transform;
            count = hitTransform.position.z - transform.position.z;
            Vector3 position = transform.position;
            for (int i = 0; i < count; i++)
            {
                position.z += 1f;
                Instantiate(bridgeTile, position, Quaternion.identity);
            }
        }
    }
}
