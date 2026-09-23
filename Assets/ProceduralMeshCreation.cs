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
    public float width = 1.0f;

    //Variable ajoute pour la partie C
    public ushort res = 1;

    private ushort nb_vertices_par_face;
    private ushort nb_vertices;
    private ushort nb_triangles_par_face;
    private ushort nb_triangles;

    private int indexTriangle;


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
    //Nouveau start partie C sans ancien start construction ancien cube
    void Start()
    {
        CreerCubeMultiRes();

        p_mesh.RecalculateBounds();

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
    //methode partie C
    private void CreerCubeMultiRes()
    {
        long nb_vertices_par_face_théoriques = (res + 1) * (res + 1);
        long nb_vertices_théoriques = nb_vertices_par_face_théoriques * 6;

        if (nb_vertices_théoriques >= ushort.MaxValue)
        {
            print("trop de vertices pour type d'indices ushort par defaut");
            print("max possible " + ushort.MaxValue + " demandé = " + nb_vertices_théoriques);

            print(
                "il faudrait modifier le type des indices avec " +
                "mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;"
            );

            return;
        }

        nb_triangles_par_face = (ushort)(2 * res * res);
        nb_vertices_par_face = (ushort)nb_vertices_par_face_théoriques;
        nb_vertices = (ushort)nb_vertices_théoriques;
        nb_triangles = (ushort)(nb_triangles_par_face * 6);

        p_mesh = new Mesh();
        p_mesh.name = "MyProceduralCubeMultiRes";

        p_vertices = new Vector3[nb_vertices];
        p_normals = new Vector3[nb_vertices];
        p_triangles = new int[nb_triangles * 3];

        //6 faces du cube
        indexTriangle = 0;

        construireFace(0, Vector3.right, Vector3.up, Vector3.back);
        construireFace(1, Vector3.left, Vector3.up, Vector3.forward);
        construireFace(2, Vector3.forward, Vector3.up, Vector3.right);
        construireFace(3, Vector3.back, Vector3.up, Vector3.left);
        construireFace(4, Vector3.right, Vector3.forward, Vector3.up);
        construireFace(5, Vector3.left, Vector3.forward, Vector3.down);

        //calcul des normales
        for (int num_face = 0; num_face < 6; num_face++)
        {
            Vector3 normalFaceEnCours =
                normaleDuTriangle(nb_triangles_par_face * num_face);

            for (int i = 0; i <= res; i++)
            {
                for (int j = 0; j <= res; j++)
                {
                    p_normals[
                        num_face * nb_vertices_par_face
                        + i * (res + 1)
                        + j
                    ] = normalFaceEnCours;
                }
            }
        }

        

        p_mesh.Clear();

        p_mesh.vertices = p_vertices;
        p_mesh.triangles = p_triangles;
        p_mesh.normals = p_normals;

        GetComponent<MeshFilter>().mesh = p_mesh;
    }

    //suite partie C
    private Vector3 normaleDuTriangle(int numTriangle)
    {
        int index = numTriangle * 3;

        Vector3 a = p_vertices[p_triangles[index]];
        Vector3 b = p_vertices[p_triangles[index + 1]];
        Vector3 c = p_vertices[p_triangles[index + 2]];

        Vector3 v1 = b - a;
        Vector3 v2 = c - a;

        return Vector3.Cross(v1, v2).normalized;
    }

    //Suite partie C
    private void construireFace(
    int numero_face,
    Vector3 axeDroit,
    Vector3 axeHaut,
    Vector3 axeProfondeur)
    {
        float decal = width / 2f;

        for (int i = 0; i <= res; i++)
        {
            for (int j = 0; j <= res; j++)
            {
                p_vertices[
                    numero_face * nb_vertices_par_face
                    + i * (res + 1)
                    + j
                ] =
                    axeHaut * (-decal + (float)i / res * width)
                    + axeDroit * (-decal + (float)j / res * width)
                    + axeProfondeur * decal;
            }
        }

        int num_vertex;

        for (int i = 0; i < res; i++)
        {
            for (int j = 0; j < res; j++)
            {
                num_vertex =
                    numero_face * nb_vertices_par_face
                    + i * (res + 1)
                    + j;

                p_triangles[indexTriangle++] = num_vertex;
                p_triangles[indexTriangle++] = num_vertex + res + 1;
                p_triangles[indexTriangle++] = num_vertex + 1;

                p_triangles[indexTriangle++] = num_vertex + res + 1;
                p_triangles[indexTriangle++] = num_vertex + res + 2;
                p_triangles[indexTriangle++] = num_vertex + 1;
            }
        }
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
