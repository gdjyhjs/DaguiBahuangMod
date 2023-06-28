using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 摄像机取样，对精灵裁剪实现可交互雪地效果
/// </summary>
public class EffectFootStepCameraCtrl : MonoBehaviour
{
    public Camera renderCamera;
    public float stepAlphaMax = .35f;
    public float stepAlphaMin = .25f;
    public float stepDuration = 5f;
    public float stepFadeAway = 1f;
    public float stepFadeIn = .3f;
    public float stepMinDistance = 1f;
    public float stepOffset = .1f;
    public float stepSpreadMax = .75f;
    public float stepSpreadMin = .65f;
    public SpriteRenderer targetRenderer;

}