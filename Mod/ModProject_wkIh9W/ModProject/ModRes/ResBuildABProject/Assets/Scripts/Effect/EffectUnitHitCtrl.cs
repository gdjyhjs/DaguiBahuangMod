using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 用于处理战斗单位受击效果拦截处理
/// </summary>
public class EffectUnitHitCtrl : MonoBehaviour
{

    #region 属性

    [Tooltip("目标死亡时替换材质，缺省不处理")]
    public string destroyMaterial = default;

    [Tooltip("目标死亡溶解替换材质，缺省不处理")]
    public string dissolveMaterial = default;

    public List<SpriteRenderer> ignoreHitSprites = new List<SpriteRenderer>();
    public bool showHitEffect = false;

    #endregion
}