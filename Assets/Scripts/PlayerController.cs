using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Material tokenMaterial;
    public InputController input;
    public HexagonController hexagonControllerPrefab;
    public int offset = 0;
    public float level = 1.0f;

    public float transitionTime = 1.0f;

    public int GetPosition()
    {
        return m_tokenPosition;
    }

    public void Advance()
    {
        m_state = State.Advancing;
        m_stateTimer = 0.0f;
    }

    public void GoBack()
    {
        m_state = State.GoingBack;
        m_stateTimer = 0.0f;
    }

    public void StayStill()
    {
        m_state = State.StayingStill;
        m_stateTimer = 0.0f;
    }

    public int GetCurrentLevel()
    {
        return m_currentLevel;
    }

	// Use this for initialization
	void Start ()
    {
        m_currentLevel = (int)level;
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

            vertices[i + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), -0.2f);

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
        m_tokenPosition = offset;

        m_tokenTemplate.hideFlags = HideFlags.HideInHierarchy;
        m_tokenTemplate.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_stateTimer > transitionTime)
        {
            if (m_state == State.Advancing)
                ++m_currentLevel;
            else if (m_state == State.GoingBack)
                --m_currentLevel;

            m_state = State.Free;
        }

        m_token.transform.localScale = new Vector3(hexagonControllerPrefab.channelWidth * 0.5f, hexagonControllerPrefab.channelWidth * 0.5f, hexagonControllerPrefab.channelWidth * 0.5f);

        if (m_state == State.Free)
        {
            float inputAngle = 0.0f;
            if (input.GetAngle(ref inputAngle))
            {
                inputAngle += Mathf.PI * 2.0f / 6.0f * (float)offset;
                m_tokenPosition = (int)Mathf.Round(((inputAngle - Mathf.PI * 0.5f) % (Mathf.PI * 2.0f)) / (Mathf.PI * 2.0f / 6.0f));
            }
        }
        else
        {
            switch (m_state)
            {
                case State.Advancing:
                    {
                        level = Mathf.Lerp(m_currentLevel, m_currentLevel + 1, m_stateTimer / transitionTime);
                    }
                    break;

                case State.GoingBack:
                    {
                        level = Mathf.Lerp(m_currentLevel, m_currentLevel - 1, m_stateTimer / transitionTime);
                    }
                    break;

                default: break;
            }

            m_stateTimer += Time.deltaTime;
        }

        float radius = hexagonControllerPrefab.GetBaseRadius() + hexagonControllerPrefab.channelWidth * 0.5f + level * (hexagonControllerPrefab.hexWidth + hexagonControllerPrefab.channelWidth);
        float angle = (float)m_tokenPosition * (Mathf.PI * 2.0f / 6.0f) + Mathf.PI * 0.5f;
        m_token.transform.position = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0.0f);
    }

    enum State
    {
        Advancing,
        GoingBack,
        StayingStill,
        Free
    }

    private int m_tokenPosition = 0;
    private GameObject m_token;
    private GameObject m_tokenTemplate;
    private State m_state = State.Free;
    private float m_stateTimer = 0.0f;
    private int m_currentLevel = 0;
}
