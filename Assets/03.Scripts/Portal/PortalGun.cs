using System;
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
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentFireRate > fireRate)
        {
            currentFireRate = 0f;
            portalIndex = 0;
            portals[portalIndex].transform.SetParent(gameObject.transform);
            portals[portalIndex].SetActive(false);
            Fire(portalIndex);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && currentFireRate > fireRate)
        {
            currentFireRate = 0f;
            portalIndex = 1;
            portals[portalIndex].transform.SetParent(gameObject.transform);
            portals[portalIndex].SetActive(false);
            Fire(portalIndex);
        }

        if (Input.GetKeyDown(KeyCode.R) && currentFireRate > fireRate)
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
        portals[portalIndex].transform.SetParent(null);
        portals[portalIndex].SetActive(true);
        Debug.Log("Hit Object: " + hit.collider.gameObject.name);
        Vector3 spawnPosition = hit.point + hit.normal * 0.5f;
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
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                CreatePortal(hit, portalIndex);
            }
        }
    }
}
