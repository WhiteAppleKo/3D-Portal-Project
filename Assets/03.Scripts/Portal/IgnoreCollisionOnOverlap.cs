using System;
using UnityEngine;

public class IgnoreCollisionOnOverlap : MonoBehaviour
{
    public string wallLayer = "Wall";
    private int targetLayer;
    public Vector3 overlapBoxSize = Vector3.one;

    private void Awake()
    {
        targetLayer = LayerMask.GetMask(wallLayer);
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, overlapBoxSize / 2, Quaternion.identity, targetLayer);
        foreach (Collider collider in colliders)
        {
            if (collider != gameObject.GetComponent<Collider>()) // 자기 자신 제외
            {
                collider.enabled = false; // 충돌하는 오브젝트의 Collider 비활성화
                Debug.Log($"Collider 비활성화: {collider.gameObject.name}");
            }
        }
    }
}