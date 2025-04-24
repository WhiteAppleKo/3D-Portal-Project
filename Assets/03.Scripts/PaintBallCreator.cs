using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBallCreator : MonoBehaviour
{
    public GameObject[] paintBallPrefabs;
    public WaitForSeconds wait;
    public float spawnTime = 0.5f;
    public int selectPaintBall;

    private void Start()
    {
        wait = new WaitForSeconds(spawnTime);
    }

    private void OnEnable()
    {
        StartCoroutine(createPaintBall(paintBallPrefabs[selectPaintBall]));
    }

    private void OnDisable()
    {
        StopCoroutine(createPaintBall(paintBallPrefabs[selectPaintBall]));
    }

    private IEnumerator createPaintBall(GameObject paintBall)
    {
        while (true)
        {
            Instantiate(paintBall, transform.position, Quaternion.identity);
            yield return wait;
        }
    }
}
