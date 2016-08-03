using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Material tokenMaterial;
    public InputController input;
    public HexagonController hexagonControllerPrefab;

	// Use this for initialization
	void Start ()
    {
        m_tokenTemplate = new GameObject();
        m_tokenTemplate.name = "Token";
        MeshFilter meshFilter = m_tokenTemplate.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = m_tokenTemplate.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        int[] indices = new int[18];
        Vector3[] vertices = new Vector3[7];

        vertices[0] = new Vector3();

        float angleStep = Mathf.PI * 2.0f / 6.0f;
        for (int i = 0; i < 6; ++i)
        {
            float angle = Mathf.PI * 0.5f + i * angleStep;

            vertices[i + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);

            indices[i * 3 + 0] = 0;
            indices[i * 3 + 1] = (i + 2);
            indices[i * 3 + 2] = (i + 1);
        }
        indices[16] = 1;

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        meshFilter.mesh = mesh;
        meshRenderer.material = tokenMaterial;

        m_token = GameObject.Instantiate(m_tokenTemplate);

        m_tokenTemplate.hideFlags = HideFlags.HideInHierarchy;
        m_tokenTemplate.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_token.transform.localScale = new Vector3(hexagonControllerPrefab.channelWidth * 0.5f, hexagonControllerPrefab.channelWidth * 0.5f, hexagonControllerPrefab.channelWidth * 0.5f);

        float inputAngle = 0.0f;
        if (input.GetAngle(ref inputAngle))
        {
            m_tokenPosition = (int)Mathf.Round(((inputAngle - Mathf.PI * 0.5f) % (Mathf.PI * 2.0f)) / (Mathf.PI * 2.0f / 6.0f));
        }

        float radius = hexagonControllerPrefab.GetBaseRadius() + hexagonControllerPrefab.channelWidth * 0.5f;
        float angle = (float)m_tokenPosition * (Mathf.PI * 2.0f / 6.0f) + Mathf.PI * 0.5f;
        m_token.transform.position = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0.0f);
	}

    private int m_tokenPosition = 0;
    private GameObject m_token;

    private GameObject m_tokenTemplate;
}
