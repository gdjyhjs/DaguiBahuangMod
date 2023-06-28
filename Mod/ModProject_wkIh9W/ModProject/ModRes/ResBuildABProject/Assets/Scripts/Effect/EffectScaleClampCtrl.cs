using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScaleClampCtrl : MonoBehaviour
{
    public float minX = 0;
    public float maxX = -1;
    public float minY = 0;
    public float maxY = -1;
    public float minZ = 0;
    public float maxZ = -1;

    private void Update()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Clamp(scale.x, minX, Mathf.Approximately(maxX, -1) ? int.MaxValue : maxX);
        scale.y = Mathf.Clamp(scale.y, minY, Mathf.Approximately(maxY, -1) ? int.MaxValue : maxY);
        scale.z = Mathf.Clamp(scale.z, minZ, Mathf.Approximately(maxZ, -1) ? int.MaxValue : maxZ);

        transform.localScale = scale;
    }
}
