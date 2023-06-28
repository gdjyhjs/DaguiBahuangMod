using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBulletBoxTypeCtrl : MonoBehaviour
{
    [SerializeField]
    public enum BulletBoxType
    {
        TriggerBox,
        MoveBox
    }

    public BulletBoxType boxType;
}
