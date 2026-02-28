using UnityEngine;

public static class GrassMeshGenerator
{
    public static Mesh Create()
    {
        Mesh mesh = new Mesh();
        mesh.name = "GrassSolid";

        float baseWidth = 0.15f;
        float midWidth = 0.08f;
        float topWidth = 0.02f;
        float height = 0.6f;
        float thickness = 0.03f;

        Vector3[] vertices = new Vector3[]
        {
            // 앞면
            new Vector3(-baseWidth, 0, thickness),
            new Vector3(baseWidth, 0, thickness),
            new Vector3(midWidth, height * 0.5f, thickness),
            new Vector3(-midWidth, height * 0.5f, thickness),
            new Vector3(topWidth, height, thickness),
            new Vector3(-topWidth, height, thickness),

            // 뒷면
            new Vector3(-baseWidth, 0, -thickness),
            new Vector3(baseWidth, 0, -thickness),
            new Vector3(midWidth, height * 0.5f, -thickness),
            new Vector3(-midWidth, height * 0.5f, -thickness),
            new Vector3(topWidth, height, -thickness),
            new Vector3(-topWidth, height, -thickness),

            // 왼쪽
            new Vector3(-baseWidth, 0, thickness),
            new Vector3(-baseWidth, 0, -thickness),
            new Vector3(-midWidth, height * 0.5f, -thickness),
            new Vector3(-midWidth, height * 0.5f, thickness),

            // 오른쪽
            new Vector3(baseWidth, 0, thickness),
            new Vector3(baseWidth, 0, -thickness),
            new Vector3(midWidth, height * 0.5f, -thickness),
            new Vector3(midWidth, height * 0.5f, thickness),

            // 바닥
            new Vector3(-baseWidth, 0, thickness),
            new Vector3(baseWidth, 0, thickness),
            new Vector3(baseWidth, 0, -thickness),
            new Vector3(-baseWidth, 0, -thickness),
        };

        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x * 0.5f + 0.5f, vertices[i].y / height);
        }

        int[] triangles = new int[]
        {
            // 앞면
            0, 1, 2, 0, 2, 3,
            3, 2, 4, 3, 4, 5,

            // 뒷면
            7, 6, 8, 8, 6, 9,
            9, 6, 10, 10, 6, 11,

            // 왼쪽
            12, 13, 14, 12, 14, 15,

            // 오른쪽
            16, 17, 18, 16, 18, 19,

            // 바닥
            20, 22, 21, 20, 23, 22,
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
}
