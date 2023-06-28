using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAutoDireCtrl : MonoBehaviour
{
    private Vector2 lastPosi = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Mathf.Approximately(lastPosi.x, transform.position.x))
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (lastPosi.x < transform.position.x ? 1 : -1), transform.localScale.y, transform.localScale.z);
        }

        lastPosi = transform.position;
    }
}
