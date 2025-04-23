using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public GameObject[] obj;

    private float a;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "ButtonTrigger")
        {
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ButtonTrigger")
        {
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].SetActive(false);
            }
        }
    }
}
