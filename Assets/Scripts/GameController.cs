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
    public RawImage blackout;

    public bool[] tokens = new bool[6];
    public int startLevel = 2;
    public float phaseTime = 3.0f;
    public int beats = 7;

    // Difficulty
    public float phaseMultiplier = 0.96f;
    public int PhaseMultiplierRythm = 1;
    public int tokenAdditionStartAt = 10;
    public int tokenAdditionRythm = 10;

    public float eyeMaxSize = 1.1f;
    public float eyeMinSize = 0.3f;
    public float pulseSize = 1.1f;
    public float pulseTime = 0.2f;
    public float blackoutTime = 0.5f;

    // Music
    public GameObject moveSoundPrefab;
    public GameObject hexAdvanceSoundPrefab;
    public GameObject musicPrefab;
    public int musicPhaseCount = 4;

    // Use this for initialization
    void Start ()
    {
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
    void Update()
    {
        if (Time.timeSinceLevelLoad < blackoutTime)
        {
            Color c = blackout.color;
            c.a = Mathf.Lerp(1f, 0f, Ease.QuadIn(Time.timeSinceLevelLoad / blackoutTime));
            blackout.color = c;
            return;
        }

        if (m_restartRequired)
        {
            float time = Time.timeSinceLevelLoad - m_restartRequestTime;
            Color c = blackout.color;
            c.a = Mathf.Lerp(0f, 1f, Ease.QuadIn(time / blackoutTime));
            blackout.color = c;

            if (time > blackoutTime)
            {
                // RESTART
                SceneManager.LoadScene("Main");
            }
            return;
        }

        // RESTART REQUEST
        if (m_gameOver)
        {
            bool everyTokenDestroyed = true;
            for (int i = 0; i < 6; ++i)
            {
                if (m_tokens[i])
                    everyTokenDestroyed = false;
            }

            if (everyTokenDestroyed && Input.anyKeyDown)
            {
                m_restartRequired = true;
                m_restartRequestTime = Time.timeSinceLevelLoad;
            }
        }

        float beatTime = phaseTime / beats;

        // DESTROY EVERY TOKEN
        if (m_gameOver)
        {
            if (m_explosionTimer < 0.0f || m_explosionTimer > beatTime)
            {
                int lowerLevel = 99999;
                int chosen = -1;
                for (int i = 0; i < 6; ++i)
                {
                    if (m_tokens[i] && m_tokens[i].GetCurrentLevel() < lowerLevel)
                    {
                        lowerLevel = m_tokens[i].GetCurrentLevel();
                        chosen = i;
                    }
                }

                if (chosen >= 0)
                {
                    m_tokens[chosen].Explode();
                    m_tokens[chosen] = null;
                }
                else
                {
                    // true end
                }

                if (m_explosionTimer > 0.0f)
                    m_explosionTimer -= beatTime;
            }
            m_explosionTimer += Time.deltaTime;
        }

        // MOVE SOUND
        if (!m_gameOver)
        {
            bool needSound = false;

            for (int i = 0; i < 6; i++)
            {
                if (m_tokens[i] && m_tokens[i].JustStartedMoving())
                    needSound = true;
            }

            if (needSound)
                Instantiate(moveSoundPrefab);
        }

        levelController.transitionTime = beatTime;
        for (int i = 0; i < 6; ++i)
        {
            if (m_tokens[i]) m_tokens[i].transitionTime = beatTime;
        }

        if (m_timer > phaseTime)
        {
            m_timer -= phaseTime;

            ++m_musicCounter;

            if (!m_gameOver)
                m_ready = true;
        }

        if (m_musicCounter >= musicPhaseCount)
        {
            GameObject go = (GameObject)Instantiate(musicPrefab);
            m_musicAudioSource = go.GetComponent<AudioSource>();
            m_musicCounter = 0;
        }

        m_musicAudioSource.pitch = m_musicAudioSource.clip.length / (phaseTime * musicPhaseCount);

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
                Instantiate(hexAdvanceSoundPrefab);
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
                            m_gameOver = true;
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

            if (!m_gameOver)
            {
                ++m_score;
                UpdateDifficuty();
            }
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
        while (scoreText.text.Length < 3)
        {
            string text = scoreText.text;
            text = text.Insert(0, "0");
            scoreText.text = text;
        }
	}

    void UpdateDifficuty()
    {
        if (m_score % PhaseMultiplierRythm == 0)
        {
            phaseTime *= phaseMultiplier;
        }

        if (m_score == tokenAdditionStartAt || (m_score > tokenAdditionStartAt && (m_score - tokenAdditionStartAt) % tokenAdditionRythm == 0))
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
    private bool m_gameOver = false;
    private float m_timer = 0.0f;
    private PlayerController[] m_tokens;
    private int m_score = 0;
    private float m_explosionTimer = -0.0001f;
    private bool m_restartRequired = false;
    private float m_restartRequestTime = 0f;

    private int m_musicCounter = 256;
    private AudioSource m_musicAudioSource = null;
}
