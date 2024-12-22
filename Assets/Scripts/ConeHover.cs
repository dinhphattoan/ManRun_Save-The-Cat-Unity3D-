using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ConeHover : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private SO_MechanicSetting playerSetting;

    private Mesh mesh;

    private void Start()
    {
        mesh = CreateConeMesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
        var meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }


    private Mesh CreateConeMesh()
    {
        if (playerSetting.HoverDistance == 0)
        {
            Debug.LogError("Hover distance in PlayerSetting is zero!");
            return new Mesh();
        }

        var centerDestination = playerSetting.HoverDistance * transform.parent.forward;
        var vertices = GenerateRoundEdge(centerDestination, playerSetting.Subdivisions, playerSetting.HoverAngle);

        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        // Triangles
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        // UVs
        for (int i = 0; i < vertices.Count; i++)
        {
            if (i == 0)
            {
                uvs.Add(new Vector2(0.5f, 1f));
            }
            else
            {
                float angle = Mathf.Atan2(vertices[i].z, vertices[i].x);
                float u = (angle + Mathf.PI) / (2 * Mathf.PI);
                float v = 0;
                uvs.Add(new Vector2(u, v));
            }
        }

        var newMesh = new Mesh();
        newMesh.vertices = vertices.ToArray();
        newMesh.triangles = triangles.ToArray();
        newMesh.uv = uvs.ToArray();
        newMesh.RecalculateNormals();

        return newMesh;
    }

    private List<Vector3> GenerateRoundEdge(Vector3 centerDestination, int subdivisions, float sideAngle)
    {
        if (centerDestination == Vector3.zero)
        {
            Debug.LogError("Center destination is zero!");
            return new List<Vector3>();
        }

        float anglePerDivision = sideAngle / subdivisions;
        sideAngle /= 2f;
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(Vector3.zero);
        // First Left side of the edge
        Quaternion rotationLeft = Quaternion.Euler(0, -sideAngle, 0);
        Vector3 leftDestination = rotationLeft * centerDestination;
        vertices.Add(leftDestination);

        // First Right side of the edge
        Quaternion rotationRight = Quaternion.Euler(0, sideAngle, 0);
        Vector3 rightDestination = rotationRight * centerDestination;

        for (int i = 0; i < subdivisions; i++)
        {
            float angle = anglePerDivision * i;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 vertex = rotation * leftDestination;
            vertices.Add(vertex);
        }

        vertices.Add(rightDestination);
        return vertices;
    }
}