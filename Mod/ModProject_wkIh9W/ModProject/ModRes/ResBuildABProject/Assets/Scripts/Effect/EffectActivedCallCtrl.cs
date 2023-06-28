using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectActivedCallCtrl : MonoBehaviour
{
    public Action onEnableCall;
    public Action onDisableCall;

    private void OnEnable()
    {
        onEnableCall?.Invoke();
    }

    private void OnDisable()
    {
        onDisableCall?.Invoke();
    }
}
