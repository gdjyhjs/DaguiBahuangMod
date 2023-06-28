using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EffectChainMoveCtrl : MonoBehaviour
{
    private class ChainItem
    {
        /// <summary>
        /// 链条
        /// </summary>
        public Transform t;                 

        /// <summary>
        /// 链条辅助节点结束位置
        /// </summary>
        public Transform tEndPosi;      
    }

    public Transform rootChain;
    public Transform move;
    public float angleMoveScale = 0;

    /// <summary>
    /// 看向某个点
    /// </summary>
    public static Quaternion LookAtPosi(Vector3 origi, Vector3 target, Vector3 scale)
    {
        float y = target.y - origi.y;
        float h = target.x - origi.x;

        float angle = Mathf.Atan2(h, y) * Mathf.Rad2Deg;//用这种

        Quaternion qua = Quaternion.Euler(-angle * scale.x, -angle * scale.y, -angle * scale.z);

        return qua;
    }
}
