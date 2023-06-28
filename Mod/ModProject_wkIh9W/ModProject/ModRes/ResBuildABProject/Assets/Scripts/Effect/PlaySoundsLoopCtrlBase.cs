using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaySoundsLoopCtrlBase : MonoBehaviour
{
    public float intervalTime = 1f;
    public float maxTime = -1;
    public float delayTime = 0;

    protected abstract string GetPath();
}
