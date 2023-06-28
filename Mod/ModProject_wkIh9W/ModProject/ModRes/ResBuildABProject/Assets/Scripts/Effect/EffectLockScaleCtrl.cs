using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLockScaleCtrl : MonoBehaviour, IUpdateEffectPosi
{
    public bool lockX = true;
    public bool lockY = true;
    public bool lockZ = true;
    public bool isWorldInParent;
    public Vector3 localScale = new Vector3(1, 1, 1);

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
        Vector3 scale = transform.localScale;

        if (isWorldInParent)
        {
            if (lockX)
            {
                scale.x = localScale.x / Mathf.Abs(transform.parent.localScale.x);
            }
            if (lockY)
            {
                scale.y = localScale.y / Mathf.Abs(transform.parent.localScale.y);
            }
            if (lockZ)
            {
                scale.z = localScale.z / Mathf.Abs(transform.parent.localScale.z);
            }

            transform.localScale = scale;
        }
        else
        {
            if (lockX)
            {
                scale.x = localScale.x;
            }
            if (lockY)
            {
                scale.y = localScale.y;
            }
            if (lockZ)
            {
                scale.z = localScale.z;
            }

            transform.localScale = scale;
        }
    }
}
