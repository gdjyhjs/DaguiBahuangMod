using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 刷新位置状态
/// </summary>
public interface IUpdateEffectPosi
{
    void UpdateEffectPosi();
}

[ExecuteInEditMode]
public class EffectUVBezierLineCtrl : MonoBehaviour, IUpdateEffectPosi
{
    /// <summary>
    /// 顶点数量
    /// </summary>
    public int vertexCount = 5;         
    /// <summary>
    /// 是否反转
    /// </summary>         
    public bool isReverse;              
    /// <summary>
    /// 线段根节点
    /// </summary>
    public Transform root;              

    public float delayDestroy = -1;

    public LineRenderer lineEffect;
    public Transform hitEffect;
    public GameObject box;
    public float scaleBoxY = -1;
    public Vector3 startPosiOffset;


    public void UpdateEffectPosi()
    {
    }

}
