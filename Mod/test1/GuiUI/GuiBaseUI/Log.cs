
using MelonLoader;
using UnityEngine;

public class Log
{
    public Log(string str)
    {
        MelonLogger.Msg(str);
        Debug.Log(str);
    }
}
