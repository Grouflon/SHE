using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class HexagonController : MonoBehaviour
{
    public float hexWidth = 2.0f;
    public float channelWidth = 0.5f;

    public float level = 2.0f;

	// Use this for initialization
	void Start ()
    {
        m_mesh = new Mesh();
        m_indices = new int[36];
        m_vertices = new Vector3[24];

        for (int i = 0; i < 6; ++i)
        {
            m_indices[i * 6 + 0] = i * 4 + 0;
            m_indices[i * 6 + 1] = i * 4 + 1;
            m_indices[i * 6 + 2] = i * 4 + 2;
            m_indices[i * 6 + 3] = i * 4 + 0;
            m_indices[i * 6 + 4] = i * 4 + 2;
            m_indices[i * 6 + 5] = i * 4 + 3;
        }

        m_mesh.vertices = m_vertices;
        m_mesh.SetIndices(m_indices, MeshTopology.Triangles, 0);
        GetComponent<MeshFilter>().mesh = m_mesh;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float angleStep = Mathf.PI * 2.0f / 6.0f;

        for (int i = 0; i < 6; ++i)
        {
            float angleStart = angleStep * i;
            float angleStop = angleStep * (i + 1);
            float centerRadius = (level * hexWidth) - (hexWidth * 0.5f);
            float upperRadius = centerRadius + hexWidth * 0.5f - channelWidth * 0.5f;
            float lowerRadius = centerRadius - hexWidth * 0.5f + channelWidth * 0.5f;

            float cosAngleStart = Mathf.Cos(angleStart);
            float sinAngleStart = Mathf.Sin(angleStart);
            float cosAngleStop = Mathf.Cos(angleStop);
            float sinAngleStop = Mathf.Sin(angleStop);

            m_vertices[i * 4 + 0] = new Vector3(cosAngleStart * upperRadius, sinAngleStart * upperRadius, 0.0f);
            m_vertices[i * 4 + 1] = new Vector3(cosAngleStart * lowerRadius, sinAngleStart * lowerRadius, 0.0f);
            m_vertices[i * 4 + 2] = new Vector3(cosAngleStop * lowerRadius, sinAngleStop * lowerRadius, 0.0f);
            m_vertices[i * 4 + 3] = new Vector3(cosAngleStop * upperRadius, sinAngleStop * upperRadius, 0.0f);
        }
        m_mesh.vertices = m_vertices;
    }

    private Mesh m_mesh;
    private int[] m_indices;
    private Vector3[] m_vertices;
}
