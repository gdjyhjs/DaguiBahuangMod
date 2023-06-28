using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLockPositionCtrl : MonoBehaviour, IUpdateEffectPosi
{
    public bool isWorld;
    public Vector3 posi;
    public bool lockX = true;
    public bool lockY = true;
    public bool lockZ = true;

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
        if (isWorld)
        {
            Vector3 p = transform.position;
            if (lockX) p.x = posi.x;
            if (lockY) p.y = posi.y;
            if (lockZ) p.z = posi.z;

            transform.position = p; 
        }
        else
        {
            Vector3 p = transform.localPosition;
            if (lockX) p.x = posi.x;
            if (lockY) p.y = posi.y;
            if (lockZ) p.z = posi.z;

            transform.localPosition = p;
        }
    }
}
