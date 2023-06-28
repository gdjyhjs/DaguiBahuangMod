using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitCmdCtrl : EffectCmdBase 
{
    //向最近敌人靠拢（延迟，移动速度，移动时间，是否更新方向，随机大小位置，是否强制锁定玩家（可选））：0|100|1
    /// <summary>
    /// 移动速度支持表达式，-1表示当前移动速度
    /// </summary>
    public void MoveNearUnit(string value)
    {
    }

    /// <summary>
    /// 向最近敌人靠拢（移动时间，插值数值，是否移动到最近敌人位置（可选））：0.5|0.1
    /// </summary>
    public void MoveNearUnitSmoothDamp(string value)
    {
    }

    /// <summary>
    /// 停止移动（延时时间（可选））：0.5
    /// </summary>
    public void StopMove(string value)
    {
    }

    /// <summary>
    /// 禁用移动（延时时间，持续时间）：0.5|0.5
    /// </summary>
    public void DisableMove(string value)
    {
    }

    /// <summary>
    /// 移动到最近敌人当前位置，支持位置偏移（延时时间|移动时间|随机偏移量最小值|随机偏移量最大值|是否实时跟踪：0|.25|100|200）
    /// </summary>
    /// <param name="expr"></param>
    public void MoveNearUnitCurPosiRandomOffset(string expr)
    {
    }

    /// <summary>
    /// 移动到最近敌人当前位置，支持位置偏移
    /// 延时时间|移动时间|X轴偏移量|Y轴偏移量|停止时间|更新发射点（可选）|实时跟踪插帧值（可选）
    /// 0|.25f|100f|200f|0f|True|0f
    /// </summary>
    /// <param name="expr"></param>
    public void MoveNearUnitCurPosiWithOffset(string expr)
    {
    }

    /// <summary>
    /// 移动到最近敌人当前位置（延时时间，移动时间，停止时间，强制在玩家位置（可选），偏移X（可选），偏移Y（可选）：0|0.5|0.1）
    /// </summary>
    public void MoveNearUnitCurPosi(string value)
    {
    }

    /// <summary>
    /// 移动到最近敌人当前方向（延时时间，移动速度，移动时间，是否反方向（可选），是否是玩家方向（可选））
    /// </summary>
    public void MoveNearUnitCurDire(string value)
    {
    }

    /// <summary>
    /// 向前方方向移动（移动速度，移动时间）
    /// </summary>
    public void MoveFrontDire(string value)
    {
    }

    /// <summary>
    /// 固定速度移动到最新敌人当前位置（移动速度，最大移动时间）
    /// </summary>
    public void MoveNearUnitCurDireFixSpeed(string value)
    {
    }

    /// <summary>
    /// 随机自身范围位置
    /// 移动时间|停止时间|随机最短距离|随机最远距离
    /// .1|.01|300|900
    /// </summary>
    public void MoveRandomPoint(string value)
    {
    }

    /// <summary>
    /// 随机移动方向（移动速度最小值，移动速度最大值，最大移动时间）
    /// </summary>
    public void MoveRandomDireFixSpeed(string value)
    {
    }

    /// <summary>
    /// 固定速度移动到敌人当前位置（移动速度，动画参数bool值，是否进入下一个BOSS动作（可选））
    /// </summary>
    public void MoveNearUnitCurPosiRunAnimSpeed(string value)
    {
    }

    /// <summary>
    /// 移动到固定位置，1上 2下 3左 4右 5屏幕中心 0中心（位置，移动时间）
    /// </summary>
    public void MoveFixPoint(string value)
    {
    }

    /// <summary>
    /// 移动到固定位置（X，Y，移动时间）
    /// </summary>
    public void MoveFixPosition(string value)
    {
    }

    /// <summary>
    /// 移动到当前位置（移动时间，延时（可选））
    /// </summary>
    public void MovePlayerCurrentPoint(string value)
    {
    }

    /// <summary>
    /// 固定速度移动到敌人当前位置（移动速度，移动倍率，动画参数bool值，是否进入下一个BOSS动作（可选），刹车速度_刹车时间(可选），删除指定ID的弹道（可选））
    /// </summary>
    public void MoveNearUnitCurDireRunAnimDur(string value)
    {
    }

    /// <summary>
    /// 死亡（延迟，是否显示特效）：0|False
    /// </summary>
    public void UnitDie(string value)
    {
    }

    /// <summary>
    /// 隐藏脚底碰撞体（隐藏时间）：0.5
    /// </summary>
    public void HideMoveBox(string value)
    {
    }

    /// <summary>
    /// 隐藏血条（隐藏时间）：0.5
    /// </summary>
    public void HideHPUI(string value)
    {
    }

    /// <summary>
    /// 隐身（持续时间）：0.5
    /// </summary>
    public void AddHideState(string value)
    {
    }

    /// <summary>
    /// 无敌并且隐身（无敌时间，是否隐藏碰撞体（可选））：0.5
    /// </summary>
    public void AddInvincibleAndHideState(string value)
    {
    }

    /// <summary>
    /// 添加效果（效果ID）：1234
    /// </summary>
    public void AddEffect(string value)
    {   }


    /// <summary>
    /// 看向指定位置（x，y）
    /// </summary>
    public void SetDireFixPosition(string value)
    {
    }

    /// <summary>
    /// 看向最近敌人（持续时间(可选)，是否跟踪当前位置（可选），是否反方向（可选））
    /// </summary>
    public void LockAttackTargetLook(string value)
    {
    }


    /// <summary>
    /// 设置方向（是否看向左边）
    /// </summary>
    public void SetDire(string value)
    {
    }

    /// <summary>
    /// 看向最近敌人（持续时间(可选)，是否跟踪当前位置（可选），是否反方向（可选））
    /// </summary>
    public void LockNearEnemy(string value)
    {
    }

    /// <summary>
    /// 设置子弹挂点位置
    /// </summary>
    public void SetPosiBullet(string value)
    {
    }

    /// <summary>
    /// 旋转朝向最近敌人（持续时间，旋转时间（可选），旋转对象（默认unit，self为自身transform））
    /// </summary>
    public void BydyRotaNearEnemy(string value)
    {
    }

    /// <summary>
    /// 跟踪单位（单位节点）
    /// </summary>
    public void TargetUnit(string value)
    {
    }

    /// <summary>
    /// 跟踪召唤主（召唤主节点）
    /// </summary>
    public void TargetSummonUnit(string value)
    {
    }

    /// <summary>
    /// 看向最近敌人（旋转速度，持续时间）：0|100|1
    /// </summary>
    public void RotaDireNearEnemy(string value)
    {
    }

    /// <summary>
    /// 设置单位到自身（偏移X，偏移Y）：0_0
    /// </summary>
    public void SetUnitToSelfPosi(string value)
    {
    }

    /// <summary>
    /// 设置单位位置（X，Y）：0_0
    /// </summary>
    public void SetUnitPosi(string value)
    {
    }

    /// <summary>
    /// 设置单位跟踪自身（持续时间）：1
    /// </summary>
    public void SetUnitTargetSelfPosi(string value)
    {
    }

    /// <summary>
    /// 播放动画（动画名称）：Attack
    /// </summary>
    public void PlayUnitAnim(string value)
    {
    }

    /// <summary>
    /// 禁用被击效果动画
    /// </summary>
    public void DisableBeHitEffect(string value)
    {
    }

    /// <summary>
    /// 看向鼠标位置（持续时间(可选)）
    /// </summary>
    public void LockMousePosi(string value)
    {
    }

    /// <summary>
    /// 锁定动画（锁定时间）：0.5
    /// </summary>
    public void LockAnim(string value)
    {
    }

    /// <summary>
    /// 设置切换动画渐变时间（渐变时间，设置时间）：100|0.5
    /// </summary>
    public void SetCrossFadeTime(string value)
    {
    }

    /// <summary>
    /// 创建技能
    /// </summary>
    public void CreateSkill(string value)
    { }

    /// <summary>
    /// 获取数值，有公式
    /// </summary>
    private float GetMathfFloat(string value, float defV)
    {
        return 0;
    }
    /// <summary>
    /// 隐藏单位（延时时间，隐藏时间）：0|5
    /// </summary>
    public void HideUnit(string value)
    {
    }

    /// <summary>
    /// 隐藏单位（延时时间，隐藏时间）：0|5
    /// </summary>
    public void HideUnitNotInvincible(string value)
    {
    }

    private void HideUnit(string value, bool isInvincible = true)
    {
    }

}
