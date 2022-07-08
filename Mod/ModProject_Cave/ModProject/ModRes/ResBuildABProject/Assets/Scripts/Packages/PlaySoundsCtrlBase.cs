using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaySoundsCtrlBase : MonoBehaviour
{
    public bool isBG = false;
    public int loopCount = 1;
    public float delayTime = 0;
    public bool isOneEffect;
    public bool isLowPassFliter;
}
