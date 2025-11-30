using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;
    public List<GameObject> obj;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            obj = new List<GameObject>();
        }else
        {
            Destroy(gameObject);
        }
    }
    
    public void isTestedButton(bool isTested)
    {
        if (isTested)
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
        DestroyAllCubes();
        obj = new List<GameObject>();
        // 씬 전환
        SceneManager.LoadScene(str);
    }

    private void CompleteSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string targetName = "SceneChangeTrigger"; // 찾고자 하는 오브젝트 이름
        obj.Add(GameObject.Find(targetName));
        foreach (var objects in obj)
        {
            objects.SetActive(false);
        }
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

    private void DestroyAllCubes()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>(); // 오브젝트 이름으로 찾기
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Cube")
            {
                Destroy(obj);
            }
        }
    }
}