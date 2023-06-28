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
    
    
    /// <summary>
    ///  是否继承播放进度；是，下一段音频将会在上一段音频结束位置开始播放。
    /// </summary>
    [Tooltip("下一段背景音是否从当前结束时间开始播放")]
    public bool overrideTime = false; 

    protected abstract string GetPath();
}
