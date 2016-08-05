using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Level
{
    public bool door0;
    public bool door1;
    public bool door2;
    public bool door3;
    public bool door4;
    public bool door5;

    public bool GetDoor(int i)
    {
        i = (i + 6) % 6;
        switch (i)
        {
            case 0: return door0;
            case 1: return door1;
            case 2: return door2;
            case 3: return door3;
            case 4: return door4;
            case 5: return door5;
            default: return false;
        }
    }
};

public class LevelController : MonoBehaviour {

    public Level[] levels;
    public InputController input;
    public GameObject hexagonPrefab;
    public int simultaneousLevels = 4;
    public float transitionTime = 0.5f;

    public void NextLevel()
    {
        ++m_wantedLevel;
    }

    public Level GetLevel(int i)
    {
        return levels[(i + (levels.Length * 10)) % levels.Length];
    }

    public int GetCurrentLevel()
    {
        return m_currentLevel;
    }

    void Start()
    {
        m_hexagons = new List<GameObject>();

        for (int i = 0; i < simultaneousLevels; ++i)
        {
            GameObject hexagon = (GameObject)GameObject.Instantiate(hexagonPrefab, Vector3.zero, Quaternion.identity);
            HexagonController controller = hexagon.GetComponent<HexagonController>();
            controller.doors = GetLevel(m_currentLevel + i);
            controller.level = m_currentLevel + i;
            m_hexagons.Add(hexagon);
        }
    }

    void Update()
    {
        if (!Application.isPlaying)
            return;

        /*if (input.NextLevel())
        {
            ++m_wantedLevel;
        }

        if (input.PrevLevel())
        {
            --m_wantedLevel;
        }*/

        if (m_wantedLevel != m_currentLevel)
        {
            float transitionSign = Mathf.Sign(m_wantedLevel - m_currentLevel);

            if (!m_transitioning)
            {
                m_transitionTimer = 0.0f;
                m_transitioning = true;

                GameObject hexagon = (GameObject)GameObject.Instantiate(hexagonPrefab, Vector3.zero, Quaternion.identity);
                HexagonController controller = hexagon.GetComponent<HexagonController>();

                if (transitionSign > 0)
                {
                    controller.doors = GetLevel(m_currentLevel + simultaneousLevels);
                    controller.level = simultaneousLevels;
                    m_hexagons.Add(hexagon);
                }
                else
                {
                    controller.doors = GetLevel(m_currentLevel - 1);
                    controller.level = -1;
                    m_hexagons.Insert(0, hexagon);
                }
            }


            float t = Mathf.Min(m_transitionTimer / transitionTime, 1.0f);
            float analogLevel = 0.0f;
            if (transitionSign > 0.0f)
            {
                analogLevel = Mathf.Lerp(0.0f, -1.0f, Ease.QuadOut(t));
            }
            else
            {
                analogLevel = Mathf.Lerp(-1.0f, 0.0f, Ease.QuadOut(t));
            }

            for (int i = 0; i < m_hexagons.Count; ++i)
            {
                HexagonController controller = m_hexagons[i].GetComponent<HexagonController>();
                if (i == m_hexagons.Count - 1)
                {
                    if (transitionSign > 0.0f)
                    {
                        analogLevel = Mathf.Lerp(10.0f, -1.0f, Ease.QuintOut(t));
                    }
                    else
                    {
                        analogLevel = Mathf.Lerp(-1.0f, 10.0f, Ease.QuintOut(t));
                    }
                    controller.level = analogLevel + i;
                }
                else
                {
                    controller.level = analogLevel + i;
                }
            }

            if (t >= 1.0f)
            {
                m_transitioning = false;
                m_currentLevel += (int)transitionSign;

                if (transitionSign > 0)
                {
                    Destroy(m_hexagons[0]);
                    m_hexagons.RemoveAt(0);
                }
                else
                {
                    Destroy(m_hexagons[m_hexagons.Count - 1]);
                    m_hexagons.RemoveAt(m_hexagons.Count - 1);
                }
            }

            m_transitionTimer += Time.deltaTime;
        }
    }


    private bool m_transitioning = false;
    private int m_wantedLevel = 0;
    private int m_currentLevel = 0;
    private float m_transitionTimer = 0.0f;
    private List<GameObject> m_hexagons;
}
