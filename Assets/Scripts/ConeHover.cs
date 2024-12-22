using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ConeHover : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private SO_MechanicSetting mechanicSetting;

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
        if (mechanicSetting.HoverDistance == 0)
        {
            Debug.LogError("Hover distance in PlayerSetting is zero!");
            return new Mesh();
        }

        var centerDestination = mechanicSetting.HoverDistance * transform.parent.forward;
        var vertices = GetListRoundVectorEdge(centerDestination, mechanicSetting.Subdivisions, mechanicSetting.HoverAngle);

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

        var newHoverMesh = new Mesh();
        newHoverMesh.vertices = vertices.ToArray();
        newHoverMesh.triangles = triangles.ToArray();
        newHoverMesh.uv = uvs.ToArray();
        newHoverMesh.RecalculateNormals();

        return newHoverMesh;
    }

    private List<Vector3> GetListRoundVectorEdge(Vector3 centerDestination, int subDivisions, float sideAngle)
    {
        if (centerDestination == Vector3.zero)
        {
            Debug.LogError("Center destination is zero!");
            return new List<Vector3>();
        }

        float anglePerDivision = sideAngle / subDivisions;
        sideAngle /= 2f;
        List<Vector3> verticesResult = new List<Vector3>
        {
            Vector3.zero
        };
        // First Left side of the edge
        Quaternion eulerLeft = Quaternion.Euler(0, -sideAngle, 0);
        Vector3 leftEulerVertice = eulerLeft * centerDestination;
        verticesResult.Add(leftEulerVertice);

        // First Right side of the edge
        Quaternion eulerRight = Quaternion.Euler(0, sideAngle, 0);
        Vector3 rightEulerVertice = eulerRight * centerDestination;

        for (int i = 0; i < subDivisions; i++)
        {
            float angle = anglePerDivision * i;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 vertex = rotation * leftEulerVertice;
            verticesResult.Add(vertex);
        }

        verticesResult.Add(rightEulerVertice);
        return verticesResult;
    }
}