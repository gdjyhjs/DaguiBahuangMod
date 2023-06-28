using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EffectAutoSortOrderCtrl : MonoBehaviour
{
    public enum CustomBattleLayer
    {
        None = -1,
        FloorEffect = 300,
        SceneEffect = 30010,
        FullEffectTop = 30040,
    }

    public CustomBattleLayer customBattleLayer = CustomBattleLayer.None;
    public float offsetY; 
}
