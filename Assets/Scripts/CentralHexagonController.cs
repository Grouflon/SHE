using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class CentralHexagonController : MonoBehaviour
{
    public HexagonController hexagonControllerPrefab;
    public Material maskMaterial;

	// Use this for initialization
	void Start ()
    {
        m_mesh = new Mesh();
        m_indices = new int[24];
        m_vertices = new Vector3[12];

        for (int i = 0; i < 2; ++i)
        {
            m_indices[i * 12 + 0] = i * 6 + 0;
            m_indices[i * 12 + 1] = i * 6 + 3;
            m_indices[i * 12 + 2] = i * 6 + 2;
            m_indices[i * 12 + 3] = i * 6 + 0;
            m_indices[i * 12 + 4] = i * 6 + 1;
            m_indices[i * 12 + 5] = i * 6 + 3;

            m_indices[i * 12 + 6 ] = i * 6 + 2;
            m_indices[i * 12 + 7 ] = i * 6 + 5;
            m_indices[i * 12 + 8 ] = i * 6 + 4;
            m_indices[i * 12 + 9 ] = i * 6 + 2;
            m_indices[i * 12 + 10] = i * 6 + 3;
            m_indices[i * 12 + 11] = i * 6 + 5;
        }

        m_mesh.vertices = m_vertices;
        m_mesh.SetIndices(m_indices, MeshTopology.Triangles, 0);
        GetComponent<MeshFilter>().mesh = m_mesh;

        // MASK
        GameObject mask = new GameObject();
        mask.name = "Mask";
        mask.hideFlags = HideFlags.DontSave;
        MeshFilter meshFilter = mask.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = mask.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        int[] indices = new int[18];
        Vector3[] vertices = new Vector3[7];

        vertices[0] = new Vector3();

        float angleStep = Mathf.PI * 2.0f / 6.0f;
        for (int i = 0; i < 6; ++i)
        {
            float angle = Mathf.PI * 0.5f + i * angleStep;
            float radius = hexagonControllerPrefab.GetBaseRadius() + hexagonControllerPrefab.channelWidth;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, transform.position.z + 0.1f);

            indices[i * 3 + 0] = 0;
            indices[i * 3 + 1] = (i + 2);
            indices[i * 3 + 2] = (i + 1);
        }
        indices[16] = 1;

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        meshFilter.mesh = mesh;
        meshRenderer.material = maskMaterial;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float angleStep = Mathf.PI * 2.0f / 6.0f;
        float radius = hexagonControllerPrefab.GetBaseRadius();

        for (int i = 0; i < 6; ++i)
        {
            float angle = i * angleStep + angleStep * 0.5f;
            m_vertices[i * 2 + 0] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0.0f);
            m_vertices[i * 2 + 1] = new Vector3(m_vertices[i * 2 + 0].x, m_vertices[i * 2 + 0].y - Mathf.Sign(m_vertices[i * 2 + 0].y) * (hexagonControllerPrefab.hexWidth), 0.0f);
        }
        m_mesh.vertices = m_vertices;
    }

    private Mesh m_mesh;
    private int[] m_indices;
    private Vector3[] m_vertices;
}
