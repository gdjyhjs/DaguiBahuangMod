using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimEvents : MonoBehaviour
{
    /// <summary>
    /// 播放特效在脚底（特效路径|是否跟踪）
    /// </summary>
    public void PlayEffect(string value)
    {
    }

    /// <summary>
    /// 前行
    /// </summary>
    public void MoveForward(string value)
    {
    }

    /// <summary>
    /// 后退
    /// </summary>
    public void MoveBack(string value)
    {
    }

    /// <summary>
    /// 移动到最近敌人
    /// </summary>
    public void MoveEnemy(string value)
    {
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    public void AddEventOne(string str, Action<string> call)
    {
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    public void DelEventOne(string str, Action<string> call)
    {
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    public void DelEventAll(string str)
    {
    }

    /// <summary>
    /// 事件
    /// </summary>
    public void Emit(string str)
    {
    }
}
