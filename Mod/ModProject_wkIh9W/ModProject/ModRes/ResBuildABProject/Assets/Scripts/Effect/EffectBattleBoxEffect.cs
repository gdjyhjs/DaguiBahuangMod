using UnityEngine;
using System.Collections;

public class EffectBattleBoxEffect : MonoBehaviour
{
    public Collider2D box;

    public string enterEffectName;
    public string stayEffectName;
    public string exitEffectName;

    public Animator anim;
    public string enterAnimName;
    public string exitAnimName;
}
