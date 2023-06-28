using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EffectSortingGroupTargetCtrl : MonoBehaviour
{
    public SortingGroup sortingGroup;
    public GameObject goTarget;

    private Renderer ren;

    // Start is called before the first frame update
    void Start()
    {
        ren = goTarget.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ren != null)
        {
            sortingGroup.sortingOrder = ren.sortingOrder;
        }
    }
}
