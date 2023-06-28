using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectClampPositionCtrl : MonoBehaviour, IUpdateEffectPosi
{
    public bool isWorld;
    public Vector3 leftDownPosi;
    public Vector3 rightUpPosi;

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
            Vector3 posi = new Vector3(Mathf.Clamp(p.x, leftDownPosi.x, rightUpPosi.x), Mathf.Clamp(p.y, leftDownPosi.x, rightUpPosi.x), Mathf.Clamp(p.z, leftDownPosi.z, rightUpPosi.z));

            transform.position = posi; 
        }
        else
        {
            Vector3 p = transform.localPosition;
            Vector3 posi = new Vector3(Mathf.Clamp(p.x, leftDownPosi.x, rightUpPosi.x), Mathf.Clamp(p.y, leftDownPosi.x, rightUpPosi.x), Mathf.Clamp(p.z, leftDownPosi.z, rightUpPosi.z));

            transform.localPosition = posi;
        }
    }
}
