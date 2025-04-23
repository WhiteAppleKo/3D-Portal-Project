using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    public GameObject[] portals;
    public Bullet bullet;
    public GameObject bulletPrefab;
    public Transform fireTransform;
    
    private float fireRate = 0.5f;
    private float currentFireRate = 0f;
    private int portalIndex = 0;
    private Ray ray;
    public Camera rayCamera;
    
    // 이벤트 선언
    public static event Action<int> OnFire;

    private void Start()
    {
        renderTexture = new RenderTexture (Screen.width, Screen.height, 24);
        rayCamera.targetTexture = renderTexture;
    }

    void Update()
    {
        currentFireRate += Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0) && currentFireRate > fireRate)
        {
            portalIndex = 0;
            portals[portalIndex].SetActive(false);
            Fire(portalIndex);
            currentFireRate = 0f;
        }
        
        if (Input.GetKey(KeyCode.Mouse1) && currentFireRate > fireRate)
        {
            portalIndex = 1;
            portals[portalIndex].SetActive(false);
            Fire(portalIndex);
            currentFireRate = 0f;
        }

        if (Input.GetKey(KeyCode.E) && currentFireRate > fireRate)
        {
            FirePaintBall();
            currentFireRate = 0f;
        }
    }

    private void FirePaintBall()
    {
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if (Physics.Raycast(ray, out hit))
        {
            bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.FireLaunch(hit.point);
        }
    }

    private void CreatePortal(RaycastHit hit, int portalIndex)
    {
        portals[portalIndex].SetActive(true);
        Debug.Log("Hit Object: " + hit.collider.gameObject.name);
        Vector3 spawnPosition = hit.point;
        Quaternion spawnRotation = Quaternion.LookRotation(hit.normal);
        portals[portalIndex].transform.position = spawnPosition;
        portals[portalIndex].transform.rotation = spawnRotation;
        OnFire?.Invoke(portalIndex);
    }

    private RenderTexture renderTexture;
    private void Fire(int portalIndex)
    {
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out hit))
        {
            // RenderTexture를 Texture2D로 복사
            RenderTexture.active = renderTexture;
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            Debug.Log($"{texture.width}, {texture.height}");
            // 충돌 지점을 스크린 좌표로 변환
            Vector3 screenPoint = rayCamera.WorldToScreenPoint(hit.point);
            Debug.Log($"충돌 지점: {screenPoint}");
            int x = Mathf.Clamp((int)screenPoint.x, 0, texture.width - 1);
            int y = Mathf.Clamp((int)screenPoint.y, 0, texture.height - 1);
            Debug.Log($"{x}, {y}");

            // 해당 좌표의 색상 가져오기
            Color color = texture.GetPixel(x, y);

            // 메모리 해제
            Destroy(texture);
            if (color.r <= 50 / 255f && color.b <= 50 / 255f)
            {
                CreatePortal(hit, portalIndex);
            }
        }
        Debug.Log("레이가 아무것도 맞추지 못했습니다.");
    }
}
