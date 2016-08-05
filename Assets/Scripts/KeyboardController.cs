using UnityEngine;
using System.Collections;
using System;

public class KeyboardController : InputController {

    public override bool GetPosition(ref int _position)
    {
        _position = m_index;
        return true;
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ++m_index;
            m_index = m_index % 6;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            --m_index;
            m_index = (m_index + 6) % 6;
        }

    }

    private int m_index = 0;
}
