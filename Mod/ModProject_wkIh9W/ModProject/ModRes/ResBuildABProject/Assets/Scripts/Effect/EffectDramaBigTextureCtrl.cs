using System;
using System.Collections;
using UnityEngine;

public class EffectDramaBigTextureCtrl : MonoBehaviour
{
    [Header("是否已经完成")]
    public bool isCompleted;

    public Action onCompleted;

    private void Complete() => isCompleted = true;

    private IEnumerator Start()
    {
        isCompleted = false;
        yield return new WaitWhile(() => isCompleted == false);

        onCompleted?.Invoke();
    }
}