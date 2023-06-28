using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaySoundsMulType
{
    Random,
    Order
}

public class PlaySoundsCtrlMul : PlaySoundsCtrlBase
{
    public string[] paths = new string[] { "" };

    public PlaySoundsMulType mulType;

    protected override string GetPath()
    {
        return "";
    }
}
