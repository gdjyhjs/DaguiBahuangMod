using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectUnitAnimEventsSub : MonoBehaviour
{
    /// <summary>
    /// 父对象
    /// </summary>
    [HideInInspector]
    public UnitAnimEvents animEvents;           

    /// <summary>
    /// 事件
    /// </summary>
    public void Emit(string str)
    {
        if (animEvents != null)
        {
            animEvents.Emit(str);
        }
    }
}
