using UnityEngine;

/// <summary>
/// 场景部件挂有碰撞或触发物体，被攻击时不显示击中特效
/// </summary>
public class EffectBarrierIgnoreHitCtrl : MonoBehaviour
{
    public static bool Has(BarrierCtrl barrier)
    {
        if (barrier == default)
            return false;

        return barrier.GetComponent<EffectBarrierIgnoreHitCtrl>() != null;
    }
}