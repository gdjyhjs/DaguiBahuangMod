using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLockScaleDireCtrl : MonoBehaviour, IUpdateEffectPosi
{
    public bool isLeft;

    private void OnEnable()
    {
        UpdateEffectPosi();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEffectPosi();
    }

    public void UpdateEffectPosi()
    {
        Vector3 vec = transform.localScale;
        transform.localScale = new Vector3(isLeft ? -Mathf.Abs(vec.x) : Mathf.Abs(vec.x), vec.y, vec.z);
    }
}
