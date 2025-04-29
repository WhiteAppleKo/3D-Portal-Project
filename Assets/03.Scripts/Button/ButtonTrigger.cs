using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public GameObject[] AbleObj;
    public GameObject[] DisableOjb;
    public GameObject NextButton;
    
    public int buttonNumber;
    private float a;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("ButtonTrigger"))
        {
            foreach (var item in AbleObj)
            {
                if (!item.activeSelf) // 이미 활성화된 경우 호출하지 않음
                {
                    item.SetActive(true);
                    SceneChanger.Instance.isTestedButton(buttonNumber);
                }
            }

            if (NextButton != null)
            {
                NextButton.SetActive(true);
            }
            
            foreach (var item in DisableOjb)
            {
                if (item.activeSelf) // 이미 비활성화된 경우 호출하지 않음
                {
                    item.SetActive(false);
                }
            }
        }

        if (other.gameObject.name == "FinishSphere")
        {
            other.GetComponent<FinishCube>().CreatePrefab();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ButtonTrigger"))
        {
            foreach (var item in AbleObj)
            {
                if (item.activeSelf) // 이미 비활성화된 경우 호출하지 않음
                {
                    item.SetActive(false);
                }
            }
        }
    }
}
