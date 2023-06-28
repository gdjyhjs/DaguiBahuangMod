using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCtrlBase
{
}

public class UnitCtrlBaseMono : MonoBehaviour
{
    public UnitCtrlBase unit;

    public void Init(UnitCtrlBase unit)
    {
        this.unit = unit;
    }
}
