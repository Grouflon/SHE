using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class HexagonController : MonoBehaviour
{
    public float hexWidth = 2.0f;
    public float channelWidth = 0.5f;

    public bool[] doors = new bool[6];
    public float level = 0.0f;
    

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
            float angleStart = angleStep * i + Mathf.PI * 0.5f;
            float angleStop = angleStep * (i + 1) + Mathf.PI * 0.5f;
            float baseRadius = GetBaseRadius() + channelWidth;
            float lowerRadius = baseRadius + level * (hexWidth + channelWidth);
            float upperRadius = lowerRadius + hexWidth;

            float cosAngleStart = Mathf.Cos(angleStart);
            float sinAngleStart = Mathf.Sin(angleStart);
            float cosAngleStop = Mathf.Cos(angleStop);
            float sinAngleStop = Mathf.Sin(angleStop);

            Vector2 startChannelOffset = new Vector2();
            Vector2 stopChannelOffset = new Vector2();

            if (doors[i])
            {
                float angle = angleStart + Mathf.PI * 2.0f / 3.0f;
                startChannelOffset = new Vector2(Mathf.Cos(angle) * channelWidth * 0.5f, Mathf.Sin(angle) * channelWidth * 0.5f);
            }
            if (doors[(i+1) % 6])
            {
                float angle = angleStop - Mathf.PI * 2.0f / 3.0f;
                stopChannelOffset = new Vector2(Mathf.Cos(angle) * channelWidth * 0.5f, Mathf.Sin(angle) * channelWidth * 0.5f);
            }

            m_vertices[i * 4 + 0] = new Vector3(cosAngleStart * upperRadius + startChannelOffset.x, sinAngleStart * upperRadius + startChannelOffset.y, 0.0f);
            m_vertices[i * 4 + 1] = new Vector3(cosAngleStart * lowerRadius + startChannelOffset.x, sinAngleStart * lowerRadius + startChannelOffset.y, 0.0f);
            m_vertices[i * 4 + 2] = new Vector3(cosAngleStop * lowerRadius + stopChannelOffset.x, sinAngleStop * lowerRadius + stopChannelOffset.y, 0.0f);
            m_vertices[i * 4 + 3] = new Vector3(cosAngleStop * upperRadius + stopChannelOffset.x, sinAngleStop * upperRadius + stopChannelOffset.y, 0.0f);
        }
        m_mesh.vertices = m_vertices;
    }

    public float GetBaseRadius()
    {
        return (hexWidth + channelWidth * 0.5f) / Mathf.Sin(Mathf.PI / 6);
    }

    private Mesh m_mesh;
    private int[] m_indices;
    private Vector3[] m_vertices;
}
