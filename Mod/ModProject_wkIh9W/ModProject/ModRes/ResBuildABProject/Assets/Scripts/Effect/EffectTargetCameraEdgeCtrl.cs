using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTargetCameraEdgeCtrl : MonoBehaviour
{
    public enum Posi
    {
        Up,
        Down,
        Left,
        Right,
        Center,
    }

    public Posi posi;
    public Vector2 offsetPosi;
    public bool targetX = true;
    public bool targetY = true;

}
