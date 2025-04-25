using System;
using System.Collections;
using System.Collections.Generic;
using PaintCore;
using PaintIn3D;
using UnityEngine;

public class CreatableWall : MonoBehaviour
{
    private class PaintableWall
    {
        public CwPaintableMesh paintableMesh;
        public CwPaintableMeshTexture paintableTexture;
        public CwColorCounter paintableColorCounter;
    }

    private PaintableWall[] paintableWalls;
    private bool isLoaded = false;
    public GameObject[] walls;
    

    private void OnEnable()
    {
        if (isLoaded == false)
        {
            LoadData();
            isLoaded = true;
        }
        
        if (paintableWalls == null)
        {
            Debug.LogWarning("PaintableWalls array is null.");
            return;
        }

        foreach (PaintableWall paint in paintableWalls)
        {
            if (paint.paintableMesh != null)
            {
                paint.paintableMesh.enabled = true;
                Debug.Log($"Enabled paintableMesh on {paint.paintableMesh.gameObject.name}");
            }

            if (paint.paintableTexture != null)
            {
                paint.paintableTexture.enabled = true;
                Debug.Log($"Enabled paintableTexture on {paint.paintableTexture.gameObject.name}");
            }

            if (paint.paintableColorCounter != null)
            {
                paint.paintableColorCounter.enabled = true;
                Debug.Log($"Enabled paintableColorCounter on {paint.paintableColorCounter.gameObject.name}");
            }
        }
    }

    private void OnDisable()
    {
        if (paintableWalls == null)
        {
            Debug.LogWarning("PaintableWalls array is null.");
            return;
        }

        foreach (PaintableWall paint in paintableWalls)
        {
            if (paint.paintableMesh != null)
            {
                paint.paintableMesh.enabled = false;
                Debug.Log($"Disabled paintableMesh on {paint.paintableMesh.gameObject.name}");
            }

            if (paint.paintableTexture != null)
            {
                paint.paintableTexture.enabled = false;
                Debug.Log($"Disabled paintableTexture on {paint.paintableTexture.gameObject.name}");
            }

            if (paint.paintableColorCounter != null)
            {
                paint.paintableColorCounter.enabled = false;
                Debug.Log($"Disabled paintableColorCounter on {paint.paintableColorCounter.gameObject.name}");
            }
        }
    }
    
    private void LoadData()
    {
        if (walls == null || walls.Length == 0)
        {
            Debug.LogWarning("Walls array is null or empty.");
            return;
        }

        paintableWalls = new PaintableWall[walls.Length];
        for (int i = 0; i < walls.Length; i++)
        {
            paintableWalls[i] = new PaintableWall();
            paintableWalls[i].paintableMesh = walls[i].GetComponent<CwPaintableMesh>();
            paintableWalls[i].paintableTexture = walls[i].GetComponent<CwPaintableMeshTexture>();
            paintableWalls[i].paintableColorCounter = walls[i].GetComponent<CwColorCounter>();

            if (paintableWalls[i].paintableMesh == null ||
                paintableWalls[i].paintableTexture == null ||
                paintableWalls[i].paintableColorCounter == null)
            {
                Debug.LogWarning($"One or more components are missing on wall {i}: {walls[i].name}");
            }
            else
            {
                Debug.Log($"Wall {i} initialized successfully: {walls[i].name}");
            }
        }
    }
}