using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonoUpdateType
{
    Update,
    LateUpdate,
    FixedUpdate,
}

[ExecuteInEditMode]
public class EffectTraceTargetCtrl : MonoBehaviour
{
    public Transform target;
    public bool isTargetRota;                               //是否跟踪旋转
	
    [HideInInspector]
    public bool isUI = false;
    [HideInInspector]
    public bool isDisableSetNullTarget = true;              //是否禁用时停止跟踪
    [HideInInspector]
    public bool isTargetActived = false;                    //是否跟踪激活状态
    [HideInInspector]
    public MonoUpdateType updateType = MonoUpdateType.Update;                       //更新类型
}