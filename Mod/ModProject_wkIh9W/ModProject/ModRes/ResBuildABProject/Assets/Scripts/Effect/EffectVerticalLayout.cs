using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 垂直布局
/// </summary>
[ExecuteAlways]
internal class EffectVerticalLayout : MonoBehaviour
{
    #region 锚点

    public Transform centerAnchor = null;

    #endregion

    private void OnTransformChildrenChanged()
    {
        UpdateSpacing();
    }

    [ContextMenu(nameof(UpdateSpacing))]
    internal void UpdateSpacing()
    {
        var children = transform.OfType<Transform>()
            .OrderBy(w => w.GetSiblingIndex())
            .ToList();

        if (centerAnchor != default)
        {
            UpdateSpacingByCenter(children);
            return;
        }

        UpdateSpacing(children);
    }

    private void UpdateSpacing(IReadOnlyList<Transform> children)
    {
        if (CheckChildren(children) == false)
            return;

        var position = children[0].position;
        var index = 0;
        foreach (var child in children)
        {
            if (child == default)
                continue;

            float y = index * spacing;
            child.transform.position = position + new Vector3(0, y, 0);

            index++;
        }
    }

    private static bool CheckChildren(IReadOnlyList<Transform> children)
    {
        return children != null && children.Any();
    }

    private void UpdateSpacingByCenter(IReadOnlyList<Transform> children)
    {
        if (CheckChildren(children) == false)
            return;

        var position = centerAnchor.position;
        position.x = transform.position.x;

        var powOfTwo = children.Count % 2 == 0;
        if (powOfTwo)
        {
            foreach (var child in children)
            {
                if (child == default)
                    continue;

                var order = child.GetSiblingIndex();
                var dir = order % 2;
                var mul = Mathf.FloorToInt(order / 2f) + .5f;

                var y = mul * spacing;
                var f = dir == 1 ? -y : y;
                child.position = position + new Vector3(0, f);
            }
        }
        else
        {
            // 中间往两边排列，上至下。
            foreach (var child in children)
            {
                if (child == default)
                    continue;

                var index = child.GetSiblingIndex();
                if (index == 0)
                {
                    child.position = position;
                }
                else
                {
                    var dir = index % 2;
                    // 0:0,1:+1,2:-1,2:+2,3:-2; 
                    var mul = Mathf.CeilToInt(index / 2f);

                    var y = mul * spacing;
                    var f = dir == 1 ? y : -y;
                    child.position = position + new Vector3(0, f);
                }
            }
        }
    }

    void Update()
    {
        UpdateSpacing();
    }

    #region 基本参数

    public bool autoUpdate = true;
    public float spacing = 0f;

    #endregion
}