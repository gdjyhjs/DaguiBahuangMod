using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLockRotationCtrl : MonoBehaviour, IUpdateEffectPosi
{
    public bool isWorld;
    public Vector3 eulerAngles;

    private void OnEnable()
    {
        UpdateEffectPosi();
    }

    private void Start()
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
        if (isWorld)
        {
            transform.eulerAngles = eulerAngles;
        }
        else
        {
            transform.localEulerAngles = eulerAngles;
        }
    }
}
