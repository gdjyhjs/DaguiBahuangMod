using UnityEngine;

/// <summary>
/// ��������������ײ�򴥷����壬������ʱ����ʾ������Ч
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