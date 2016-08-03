using UnityEngine;
using System.Collections;

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

    void Start()
    {

    }

    void Update()
    {
        if (input.NextLevel())
        {
            Debug.Log("next");
        }

        if (input.PrevLevel())
        {
            Debug.Log("prev");
        }
    }

    private float m_currentLevel = 0.0f;
}
