using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Material tokenMaterial;
    public InputController input;
    public HexagonController hexagonControllerPrefab;
    public GameObject tokenPrefab;
    public int offset = 0;
    public float level = 1.0f;
    public float z = -0.06f;

    public float transitionTime = 1.0f;
    public float horizontalTransitionTime = 0.1f;
    public float spawningTime = 0.2f;

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
        m_tokenTemplate = Instantiate(tokenPrefab);
        m_tokenTemplate.name = "Token";
        MeshFilter meshFilter = m_tokenTemplate.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = m_tokenTemplate.AddComponent<MeshRenderer>();
        TrailRenderer trailRenderer = m_tokenTemplate.GetComponent<TrailRenderer>();
        //trailRenderer.material = tokenMaterial;
        trailRenderer.startWidth = hexagonControllerPrefab.channelWidth;
        trailRenderer.endWidth = 0.0f;//hexagonControllerPrefab.channelWidth;

        Mesh mesh = new Mesh();
        int[] indices = new int[18];
        Vector3[] vertices = new Vector3[7];

        vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);

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
        int position = 0;
        input.GetPosition(ref position);
        m_tokenPosition = (offset + position) % 6;
        m_tokenWantedPosition = m_tokenPosition;
        m_token.transform.localScale = Vector3.zero;

        m_tokenTemplate.hideFlags = HideFlags.HideInHierarchy;
        m_tokenTemplate.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        float scale = hexagonControllerPrefab.channelWidth * 0.5f;
        if (m_spawningTimer < spawningTime)
        {
            scale = Mathf.Lerp(0.0f, hexagonControllerPrefab.channelWidth * 0.5f, Ease.QuadOut(m_spawningTimer / spawningTime));
            m_spawningTimer += Time.deltaTime;
        }
        m_token.transform.localScale = new Vector3(scale, scale, scale);

        if (m_transitionTimer > horizontalTransitionTime)
        {
            m_transitionTimer = 0.0f;

            int delta = m_tokenWantedPosition - m_tokenPosition;
            int increment = (int)(Mathf.Abs(delta) > 3 ? Mathf.Sign(-delta) : Mathf.Sign(delta));
            m_tokenPosition = (m_tokenPosition + 6 + increment) % 6;
        }

        if (m_stateTimer > transitionTime)
        {
            if (m_state == State.Advancing)
                ++m_currentLevel;
            else if (m_state == State.GoingBack)
                --m_currentLevel;

            m_state = State.Free;
        }

        if (m_state == State.Free)
        {
            int position = 0;
            if (input.GetPosition(ref position))
            {
                m_tokenWantedPosition = (position + offset) % 6;
            }
        }
        else
        {
            switch (m_state)
            {
                case State.Advancing:
                    {
                        level = Mathf.Lerp(m_currentLevel, m_currentLevel + 1, Ease.QuintOut(m_stateTimer / transitionTime));
                    }
                    break;

                case State.GoingBack:
                    {
                        level = Mathf.Lerp(m_currentLevel, m_currentLevel - 1, Ease.QuintOut(m_stateTimer / transitionTime));
                    }
                    break;

                default: break;
            }

            m_stateTimer += Time.deltaTime;
        }

        float radius = hexagonControllerPrefab.GetBaseRadius() + hexagonControllerPrefab.channelWidth * 0.5f + level * (hexagonControllerPrefab.hexWidth + hexagonControllerPrefab.channelWidth);

        if (m_tokenPosition == m_tokenWantedPosition)
        {
            float angle = (float)m_tokenPosition * (Mathf.PI * 2.0f / 6.0f) + Mathf.PI * 0.5f;
            m_token.transform.position = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, z);
        }
        else
        {
            int delta = m_tokenWantedPosition - m_tokenPosition;
            int increment = (int)(Mathf.Abs(delta) > 3 ? Mathf.Sign(-delta) : Mathf.Sign(delta));

            float startAngle = (float)m_tokenPosition * (Mathf.PI * 2.0f / 6.0f) + Mathf.PI * 0.5f;
            Vector3 startPosition = new Vector3(Mathf.Cos(startAngle) * radius, Mathf.Sin(startAngle) * radius, z);

            float EndAngle = (float)(m_tokenPosition + increment)  * (Mathf.PI * 2.0f / 6.0f) + Mathf.PI * 0.5f;
            Vector3 endPosition = new Vector3(Mathf.Cos(EndAngle) * radius, Mathf.Sin(EndAngle) * radius, z);

            m_token.transform.position = Vector3.Lerp(startPosition, endPosition, Ease.QuadOut(m_transitionTimer / horizontalTransitionTime));

            m_transitionTimer += Time.deltaTime;
        }
    }

    enum State
    {
        Advancing,
        GoingBack,
        StayingStill,
        Free
    }

    private int m_tokenWantedPosition = 0;
    private int m_tokenPosition = 0;
    private float m_transitionTimer = 0.0f;

    private GameObject m_token;
    private GameObject m_tokenTemplate;
    private State m_state = State.Free;
    private float m_stateTimer = 0.0f;
    private int m_currentLevel = 0;

    private float m_spawningTimer = 0.0f;
}
