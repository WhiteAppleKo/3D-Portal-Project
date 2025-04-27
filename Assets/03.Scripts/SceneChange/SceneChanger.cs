using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;
    public bool[] isTested;
    public GameObject[] obj;
    public String currentSceneName;
    public GameObject playerPreFab;
    private GameObject player;
    private Transform playerTransform;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (playerTransform == null)
        {
            playerTransform = playerPreFab.transform;
        }

        if (player == null)
        {
            player = Instantiate(playerPreFab, playerTransform.position, playerTransform.rotation);
        }
        isTested = new bool[5];
    }
    
    public void isTestedButton(int i)
    {
        isTested[i] = true;
        if (isTested.All(test => test))
        {
            foreach (var item in obj)
            {
                item.SetActive(true);
            }
        }
        
    }

    Vector3 savedPosition;
    Quaternion savedRotation;
    public void SceneChange(String str)
    {
        savedPosition = player.transform.position;
        savedRotation = player.transform.rotation;
        SceneManager.sceneLoaded += CompleteSceneLoaded;

        // 씬 전환
        SceneManager.LoadScene(str);
    }

    private void CompleteSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬에서 플레이어 생성
        player = Instantiate(playerPreFab, savedPosition, savedRotation);
        var portals = FindObjectsOfType<Portal>().Where(portal => portal.gameObject.activeSelf);
        foreach (var portal in portals)
        {
            portal.gameObject.SetActive(false);
            portal.gameObject.SetActive(true);
            portal.playerCam = Camera.main;
        }
        // 이벤트 해제
        SceneManager.sceneLoaded -= CompleteSceneLoaded;
    }
}
