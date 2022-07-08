using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectUnitNodeCtrl : MonoBehaviour
{
    public enum UnitNodeType
    {
        None,
        Down,
        Center,
        Top,
        Bullet,
        Posi1,
        Posi2,
        Posi3,
        Posi4,
        Posi5,
    }
    public enum ScaleType
    {
        None,
        X,
        Y,
        XY,
        Max,
    }
    public enum ResetUnitNodeType
    {
        None,
        SummonUnit,         //召唤者
        EffectSource,       //效果来源者
    }

    public UnitNodeType nodeType;
    public float delayDestroyTime = -1f;
    public bool isTarget;
    public bool isTargetDire = false;
    public bool isTargetAngle = false;
    public ScaleType autoScale;
    public float maxScale = 100f;

}
