using UnityEngine;
using System.Collections;
using System;

public class GamepadController : InputController
{
    public override bool GetPosition(ref int _angle)
    {
        return false;
        /*Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (direction.magnitude < 0.4f)
            return false;

        direction.Normalize();
        _angle = Mathf.Atan2(direction.y, direction.x);
        return true;*/
    }

    /*public override bool NextLevel()
    {
        return Input.GetButtonDown("RS");
    }

    public override bool PrevLevel()
    {
        return Input.GetButtonDown("LS");
    }*/
}
