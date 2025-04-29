using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCube : MonoBehaviour
{
    public GameObject preFab;

    public void CreatePrefab()
    {
        Instantiate(preFab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
