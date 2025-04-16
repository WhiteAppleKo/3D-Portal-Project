using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateMesh : MonoBehaviour
{
    [SerializeField] int segments = 64;  // 원의 해상도
    [SerializeField] float radius = 1f;

    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Circle";

        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];
        Vector2[] uvs = new Vector2[vertices.Length];

        // 중심점
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        // 주변 점들
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            vertices[i + 1] = new Vector3(x, y, 0); // XY 평면
            uvs[i + 1] = new Vector2((x / radius + 1f) * 0.5f, (y / radius + 1f) * 0.5f);
        }

        // 삼각형 연결
        for (int i = 0; i < segments; i++)
        {
            int current = i + 1;
            int next = (i + 1) % segments + 1;

            triangles[i * 3] = 0;        // 중심점
            triangles[i * 3 + 1] = next;
            triangles[i * 3 + 2] = current;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        AssetDatabase.CreateAsset(mesh, "Assets/" + mesh.name + ".mesh");
        Debug.Log(AssetDatabase.GetAssetPath(mesh));
    }
}
