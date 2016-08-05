using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public LevelController levelController;
    public InputController input;
    public PlayerController playerControllerPrefab;
    public GameObject eye;
    public Text scoreText;

    public bool[] tokens = new bool[6];
    public int startLevel = 2;
    public float phaseTime = 3.0f;
    public int beats = 7;

    // Difficulty
    public float phaseMultiplier = 0.96f;
    public int PhaseMultiplierRythm = 1;
    public int tokenAdditionRythm = 10;

    public float eyeMaxSize = 1.1f;
    public float eyeMinSize = 0.3f;
    public float pulseSize = 1.1f;
    public float pulseTime = 0.2f;


	// Use this for initialization
	void Start ()
    {
        /*HexagonController[] hc = FindObjectsOfType<HexagonController>();
        foreach (HexagonController hex in hc)
        {
            Destroy(hex.gameObject);
        }*/

        m_tokens = new PlayerController[6];

        levelController.transitionTime = phaseTime / beats;

	    for (int i = 0; i < 6; ++i)
        {
            if (tokens[i])
            {
                PlayerController controller = (PlayerController)Instantiate(playerControllerPrefab, Vector3.zero, Quaternion.identity);
                controller.offset = i;
                controller.input = input;
                controller.level = startLevel;
                controller.transitionTime = phaseTime / beats;
                m_tokens[i] = controller;
            }
            else
            {
                m_tokens[i] = null;
            }
        }

        SetEyeSize(eyeMinSize);
    }
	
	// Update is called once per frame
	void Update ()
    {
        float beatTime = phaseTime / beats;

        levelController.transitionTime = beatTime;
        for (int i = 0; i < 6; ++i)
        {
            if (m_tokens[i]) m_tokens[i].transitionTime = beatTime;
        }

        if (m_timer > phaseTime)
        {
            m_timer -= phaseTime;
            m_ready = true;
        }

        int currentBeat = Mathf.FloorToInt(m_timer / beatTime);

        if (currentBeat >= (beats - 1) && m_ready)
        {
            m_ready = false;
            bool[] passing = new bool[6];

            bool nonePassing = true;
            bool oneExiting = false;
            for (int i = 0; i < 6; ++i)
            {
                if (!m_tokens[i])
                {
                    passing[i] = false;
                    continue;
                }

                Level level = levelController.GetLevel(levelController.GetCurrentLevel() + m_tokens[i].GetCurrentLevel());
                passing[i] = level.GetDoor(m_tokens[i].GetPosition());
                if (passing[i])
                {
                    nonePassing = false;
                    if (m_tokens[i].GetCurrentLevel() == levelController.simultaneousLevels - 1)
                    {
                        oneExiting = true;
                    }
                }
            }

            if (nonePassing || oneExiting)
            {
                levelController.NextLevel();
                for (int i = 0; i < 6; ++i)
                {
                    if (!m_tokens[i])
                        continue;

                    if (passing[i]) m_tokens[i].StayStill();
                    if (!passing[i])
                    {
                        m_tokens[i].GoBack();

                        if (m_tokens[i].GetCurrentLevel() == 0)
                        {
                            // GAME OVER
                            SceneManager.LoadScene("Main");
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 6; ++i)
                {
                    if (!m_tokens[i])
                        continue;

                    if (passing[i]) m_tokens[i].Advance();
                }
            }

            ++m_score;
            UpdateDifficuty();
        }

        float modTime = m_timer % phaseTime;
        float modModTime = modTime % beatTime;

        float pulseMod = 1.0f;
        if (modModTime < pulseTime)
        {
            float t1 = modModTime / pulseTime;
            //float t = (t1 < 0.5f) ? (t1 * 2.0f) : (1.0f - (t1 - 0.5f) * 2.0f);
            float t = t1;
            pulseMod = Mathf.Lerp(pulseSize, 1.0f, t);
        }

        if (currentBeat < beats - 1)
        {
            SetEyeSize(Mathf.Lerp(eyeMinSize, eyeMaxSize, Ease.QuadIn(modTime / (beatTime * (beats - 1)))) * pulseMod);
        }
        else
        {
            SetEyeSize(Mathf.Lerp(eyeMaxSize, eyeMinSize, Ease.QuintOut(modModTime / beatTime)) * pulseMod);
        }

        m_timer += Time.deltaTime;
        scoreText.text = m_score.ToString();
	}

    void UpdateDifficuty()
    {
        if (m_score % PhaseMultiplierRythm == 0)
        {
            phaseTime *= phaseMultiplier;
        }

        if (m_score % tokenAdditionRythm == 0)
        {
            List<int> availableSlots = new List<int>();
            for (int i = 0; i < 6; ++i)
            {
                if (!m_tokens[i])
                    availableSlots.Add(i);
            }

            if (availableSlots.Count > 0)
            {
                int i = Random.Range(0, availableSlots.Count - 1);
                int slot = availableSlots[i];
                
                m_tokens[slot] = (PlayerController)Instantiate(playerControllerPrefab, Vector3.zero, Quaternion.identity);
                m_tokens[slot].offset = slot;
                m_tokens[slot].level = startLevel;
                m_tokens[slot].input = input;
                m_tokens[slot].transitionTime = phaseTime / beats;
            }
        }
    }

    void SetEyeSize(float _size)
    {
        eye.transform.localScale = new Vector3(_size, eye.transform.localScale.z, _size);
    }

    private bool m_ready = true;
    private float m_timer = 0.0f;
    private PlayerController[] m_tokens;
    private int m_score = 0;
}
