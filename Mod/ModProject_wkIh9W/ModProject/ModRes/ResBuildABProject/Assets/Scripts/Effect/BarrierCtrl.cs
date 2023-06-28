using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarrierCtrl : MonoBehaviour
{
    public EffectAutoSortOrderCtrl.CustomBattleLayer battleLayer = EffectAutoSortOrderCtrl.CustomBattleLayer.None;
    public float offsetY;
    public List<Vector2Int> points = new List<Vector2Int>();
}
