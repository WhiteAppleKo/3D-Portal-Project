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
    public GameObject[] walls;
    

    private void OnEnable()
    {
        LoadData();
        
        if (paintableWalls == null)
        {
            return;
        }

        foreach (PaintableWall paint in paintableWalls)
        {
            if (paint.paintableMesh != null)
            {
                paint.paintableMesh.enabled = true;
            }

            if (paint.paintableTexture != null)
            {
                paint.paintableTexture.enabled = true;
            }

            if (paint.paintableColorCounter != null)
            {
                paint.paintableColorCounter.enabled = true;
            }
        }
        foreach (var item in walls)
        {
            item.SetActive(false);
            item.SetActive(true);
        }
    }

    // private void OnDisable()
    // {
    //     if (paintableWalls == null)
    //     {
    //         return;
    //     }
    //
    //     foreach (PaintableWall paint in paintableWalls)
    //     {
    //         if (paint.paintableMesh != null)
    //         {
    //             paint.paintableMesh.enabled = false;
    //         }
    //
    //         if (paint.paintableTexture != null)
    //         {
    //             paint.paintableTexture.enabled = false;
    //         }
    //
    //         if (paint.paintableColorCounter != null)
    //         {
    //             paint.paintableColorCounter.enabled = false;
    //         }
    //     }
    //     paintableWalls = null;
    // }
    
    private void LoadData()
    {
        if (walls == null || walls.Length == 0)
        {
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
            }
        }
    }
}