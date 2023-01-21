using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assure que l'objet associé au script possède bien un Mesh filter
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    public int xSize = 20;
    public int zSize = 20;

    // rayon de déformation
    public float deformRadius = 0.2f;

    // deformation maximal causé par une collision
    public float maxDeform = 0.1f;

    //
    public float damageFallOff = 1;

    // dommage minimum pouvant causer une défomation
    public float minDamage = 1;

    // quantité de dommage par collision
    public float damageMultiplier = 1;

    private MeshCollider col;
    private MeshFilter meshFilter;
    private Vector3[] startingVertices;

    void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = mesh;
        col = GetComponent<MeshCollider>();
        startingVertices = meshFilter.mesh.vertices;
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize +1) * (zSize +1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .4f, z * .4f);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        // keeps a trace of the vertice we are currently looking at
        int vert = 0;
        // same for the triangle
        int triangle = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[triangle] = vert + 0;
                // xsize +1 because vertices are ordered from left to right
                triangles[triangle + 1] = vert + xSize + 1;
                triangles[triangle + 2] = vert + 1;
                triangles[triangle + 3] = vert + 1;
                triangles[triangle + 4] = vert + xSize + 1;
                triangles[triangle + 5] = vert + xSize + 2;
                vert++;
                triangle += 6;
            }
            vert++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float collisionPower = collision.impulse.magnitude;

        if (collisionPower > minDamage)
        {
            // chaque point de collision du mesh
            foreach (ContactPoint point in collision.contacts)
            {
                // vérification des sommets touchés
                for (int i = 0; i < vertices.Length; i++) // slow method but complete and good for low poly
                {
                    Vector3 vertexPosition = vertices[i];
                    // world space to local space
                    Vector3 pointPosition = transform.InverseTransformPoint(point.point);
                    float distanceFromCollision = Vector3.Distance(vertexPosition, pointPosition);
                    // assure que le vertex n'est pas trop loin lorsqu'il se déforme
                    float distanceFromOriginal = Vector3.Distance(vertexPosition, startingVertices[i]);

                    if (distanceFromCollision < deformRadius && distanceFromOriginal < maxDeform) // If within collision radius and within max deform
                    {
                        float falloff = 1 - (distanceFromCollision / deformRadius) * damageFallOff;

                        float xDeform = pointPosition.x * falloff;
                        float yDeform = pointPosition.y * falloff;
                        float zDeform = pointPosition.z * falloff;

                        xDeform = Mathf.Clamp(xDeform, 0, maxDeform);
                        yDeform = Mathf.Clamp(yDeform, 0, maxDeform);
                        zDeform = Mathf.Clamp(zDeform, 0, maxDeform);

                        Vector3 deform = new Vector3(xDeform, yDeform, zDeform);
                        vertices[i] -= deform * damageMultiplier;
                    }
                }
            }
            UpdateMesh();
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        startingVertices = meshFilter.mesh.vertices;
        mesh.triangles = triangles;
        col.sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        for(int i=0; i< vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
