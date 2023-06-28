using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundsCtrl : PlaySoundsCtrlBase
{
    public string path;

    protected override string GetPath()
    {
        return path;
    }
}
