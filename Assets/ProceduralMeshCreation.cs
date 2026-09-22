
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class ProceduralMeshCreation : MonoBehaviour
{

    private Mesh p_mesh;
    private Vector3[] p_vertices;
    private int[] p_triangles;
    private Vector3[] p_normals;
    private void DebugNormals()
    {
        for (int num_vert = 0; num_vert < p_vertices.Length; num_vert++)
        {
            Debug.DrawRay(transform.position + p_vertices[num_vert],
            p_normals[num_vert].normalized * 2.0f,
            Color.red, 30);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        p_mesh = new Mesh();

        p_vertices = new Vector3[7];
        p_vertices[0] = new Vector3(0, 0, 0);
        p_vertices[1] = new Vector3(0, 1, 0);
        p_vertices[2] = new Vector3(1, 1, 0);
        p_vertices[3] = new Vector3(1, 0, 0);
        p_vertices[4] = new Vector3(0, 1, 1);
        p_vertices[5] = p_vertices[1];
        p_vertices[6] = p_vertices[2];



        p_triangles = new int[]
        {
            0, 1, 2 ,
            0, 2, 3,
            5, 4, 6,
        };

        p_normals = new Vector3[p_vertices.Length];

        Vector3 V1 = p_vertices[p_triangles[1]] - p_vertices[p_triangles[0]];
        Vector3 V2 = p_vertices[p_triangles[2]] - p_vertices[p_triangles[0]];

        Vector3 N = Vector3.Cross(V1, V2).normalized;

        p_normals[p_triangles[0]] = N;
        p_normals[p_triangles[1]] = N;
        p_normals[p_triangles[2]] = N;
        p_normals[3] = N;
        p_normals[4] = new Vector3(0, 1, 0);
        p_normals[5] = new Vector3(0, 1, 0);
        p_normals[6] = new Vector3(0, 1, 0);

        p_mesh.Clear();

        p_mesh.vertices = p_vertices;
        p_mesh.triangles = p_triangles;
        p_mesh.normals = p_normals;
        p_mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = p_mesh;
        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = p_mesh;

        DebugNormals();

        float surface = CalculerSurface();

        Debug.Log(
            "Objet : " + gameObject.name +
            " : " + p_vertices.Length + " vertices, " +
            (p_triangles.Length / 3) + " triangles, surface : " +
            surface
        );

    }

    private void DebugAllNormals(bool affN_Orientation, bool affN_Eclairage, bool affN_vertices)
    {
        if (affN_Orientation)
        {
            Vector3 vposmoyenne;
            Vector3 v1;
            Vector3 v2;
            Vector3 vnormetri;

            for (int num_tri = 0; num_tri < p_triangles.Length; num_tri += 3)
            {
                vposmoyenne =
                    p_vertices[p_triangles[num_tri]]
                    + p_vertices[p_triangles[num_tri + 1]]
                    + p_vertices[p_triangles[num_tri + 2]];

                vposmoyenne /= 3.0f;

                v1 = p_vertices[p_triangles[num_tri + 1]]
                    - p_vertices[p_triangles[num_tri]];

                v2 = p_vertices[p_triangles[num_tri + 2]]
                    - p_vertices[p_triangles[num_tri]];

                vnormetri = Vector3.Cross(v1, v2);

                Debug.DrawRay(
                    transform.position + vposmoyenne,
                    vnormetri.normalized * 3.0f,
                    Color.yellow,
                    5,
                    false
                );
            }
        }

        if (affN_Eclairage)
        {
            Vector3 vposmoyenne;
            Vector3 vnormmoyenne;

            for (int num_tri = 0; num_tri < p_triangles.Length; num_tri += 3)
            {
                vposmoyenne =
                    (
                        p_vertices[p_triangles[num_tri]]
                        + p_vertices[p_triangles[num_tri + 1]]
                        + p_vertices[p_triangles[num_tri + 2]]
                    ) / 3.0f;

                vnormmoyenne =
                    (
                        p_normals[p_triangles[num_tri]]
                        + p_normals[p_triangles[num_tri + 1]]
                        + p_normals[p_triangles[num_tri + 2]]
                    ) / 3.0f;

                Debug.DrawRay(
                    transform.position + vposmoyenne,
                    vnormmoyenne.normalized * 2.0f,
                    Color.green,
                    5,
                    false
                );
            }
        }

        if (affN_vertices)
        {
            for (int num_vert = 0; num_vert < p_vertices.Length; num_vert++)
            {
                Debug.DrawRay(
                    transform.position + p_vertices[num_vert],
                    p_normals[num_vert].normalized * 1.0f,
                    Color.red,
                    5,
                    false
                );
            }
        }
    }

    private float CalculerSurface()
    {
        float surfaceTotale = 0.0f;

        for (int i = 0; i < p_triangles.Length; i += 3)
        {
            Vector3 a = p_vertices[p_triangles[i]];
            Vector3 b = p_vertices[p_triangles[i + 1]];
            Vector3 c = p_vertices[p_triangles[i + 2]];

            Vector3 v1 = b - a;
            Vector3 v2 = c - a;

            Vector3 produitVectoriel = Vector3.Cross(v1, v2);

            float surfaceTriangle = produitVectoriel.magnitude / 2.0f;

            surfaceTotale += surfaceTriangle;
        }

        return surfaceTotale;
    }


    // Update is called once per frame
    void Update()
    {
        DebugAllNormals(true, true, true);

    }
}
