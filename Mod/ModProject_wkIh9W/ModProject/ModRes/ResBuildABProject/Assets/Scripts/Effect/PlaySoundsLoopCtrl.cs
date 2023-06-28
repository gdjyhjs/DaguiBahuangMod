using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundsLoopCtrl : PlaySoundsLoopCtrlBase
{
    public string path;

    protected override string GetPath()
    {
        return path;
    }
}
