using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBoxBlockEnemyCtrl : MonoBehaviour, ISetUnitMono
{
    [HideInInspector]
    public UnitCtrlBaseMono unitMono;

    public void SetUnitMono(UnitCtrlBaseMono unitMono)
    {
    }
}
