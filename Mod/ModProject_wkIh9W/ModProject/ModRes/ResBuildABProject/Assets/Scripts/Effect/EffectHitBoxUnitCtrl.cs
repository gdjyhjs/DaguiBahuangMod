using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHitBoxUnitCtrl : MonoBehaviour
{
    private Dictionary<int, UnitCtrlBase> hitUnits = new Dictionary<int, UnitCtrlBase>();
    public float clearUnitsIntervalTime = 0.15f;

    private float curTime;

    private void OnDisable()
    {
        hitUnits.Clear();
        curTime = 0;
    }

    private void Update()
    {
        curTime += Time.deltaTime;

        if (curTime >= clearUnitsIntervalTime)
        {
            hitUnits.Clear();
            curTime = 0;
        }
    }

    public void AddUnit(UnitCtrlBase u)
    {
        if (u == null)
        {
            return;
        }
        if (!hitUnits.ContainsKey(u.GetHashCode()))
        {
            hitUnits[u.GetHashCode()] = u;
        }
    }

    public bool IsContainsUnit(UnitCtrlBase u)
    {
        if (u == null)
        {
            return false;
        }
        return hitUnits.ContainsKey(u.GetHashCode());
    }
}
