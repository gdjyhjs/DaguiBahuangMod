using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 设置单位
/// </summary>
public interface ISetUnitMono
{
    void SetUnitMono(UnitCtrlBaseMono unitMono);
}

public class EffectCmdBase : MonoBehaviour, ISetUnitMono
{
    public UnityEvent onStartCall = new UnityEvent();
    public UnityEvent onDestroyCall = new UnityEvent();
    public UnityEvent onDestroyEffectCall = new UnityEvent();
    public UnityEvent onHitUnitCall = new UnityEvent();
    public UnityEvent onUnitDieCall = new UnityEvent();

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
    }

    protected virtual void InitUnitMono()
    {
    }

    public void HitUnit(UnitCtrlBase unit)
    {
    }

    private void OnUnitDie()
    {
    }

    public void SetUnitMono(UnitCtrlBaseMono unitMono)
    {
    }

    public void StartCall()
    {
        onStartCall?.Invoke();
    }

    public void DestroyCall()
    {
        onDestroyCall?.Invoke();
    }

    public void HitUnitCall()
    {
        onHitUnitCall?.Invoke();
    }

    public void UnitDieCall()
    {
        onUnitDieCall?.Invoke();
    }
}
