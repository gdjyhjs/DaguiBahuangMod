using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroyEffectTypesCtrl : MonoBehaviour
{
    public enum DestroyType
    {
        None,
        StopEmitting,
        DelayTime,
    }

    public DestroyType destroyType;

    public float stopEmittingDelayTime;
    public string stopPlayAnim;
}
