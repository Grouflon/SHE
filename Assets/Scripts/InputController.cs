using UnityEngine;
using System.Collections;

public abstract class InputController : MonoBehaviour
{
    public abstract bool GetAngle(ref float _angle);

    public abstract bool NextLevel();
    public abstract bool PrevLevel();
}
