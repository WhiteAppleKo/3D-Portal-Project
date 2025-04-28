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

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
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
        SceneManager.sceneLoaded += CompleteSceneLoaded;

        // 씬 전환
        SceneManager.LoadScene(str);
    }

    private void CompleteSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var portals = FindObjectsOfType<Portal>().Where(portal => portal.gameObject.activeSelf);
        foreach (var portal in portals)
        {
            portal.gameObject.SetActive(false);
            portal.gameObject.SetActive(true);
            portal.playerCam = Camera.main;
        }
        isTested = new bool[5];
        // 이벤트 해제
        SceneManager.sceneLoaded -= CompleteSceneLoaded;
    }
}