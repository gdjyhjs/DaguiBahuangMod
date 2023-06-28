using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D.Animation;
using System.Reflection;

public class EffectCmdCtrl : EffectCmdBase
{
    /// <summary>
    /// 控制全局光（延迟，亮度，持续时间）：0.5|0|1
    /// </summary>
    public void LightDOIntensity(string value)
    {
    }

    /// <summary>
    /// 缩放时间（延迟，缩放值，缩放时间）：0.5|0.1|1
    /// </summary>
    public void TimeScale(string value)
    {
    }

    /// <summary>
    /// 缩放时间（延迟，缩放值，缩放时间）：0.5|0.1|1
    /// </summary>
    public void UntimeScale(string value)
    {
    }

    /// <summary>
    /// 移动镜头到某个位置（延迟，移动速度，位置X，位置Y，停留时间，镜头缩放值：0.5|10|5|5|0.5|5.4
    /// </summary>
    public void MoveCamera(string value)
    {
    }

    /// <summary>
    /// 缩放镜头（延迟，缩放值，缩放时间，是否跟踪到当前位置（可选），跟踪偏移位置X（可选），跟踪偏移位置Y（可选））：0.5|0.1|1
    /// </summary>
    public void ScaleCameraOrthoSize(string value)
    {
    }

    /// <summary>
    /// 延时销毁（延时时间）
    /// </summary>
    public void DelayDestroy(float delay)
    {
    }

    /// <summary>
    /// 播放动画（延时时间，动画名称）：0.5|Idle
    /// </summary>
    public void PlayAnim(string value)
    {
    }

    /// <summary>
    /// 人物遮罩（延时时间，是否打开）：0.5|True
    /// </summary>
    public void PlayerMask(string value)
    {
    }

    /// <summary>
    /// 人物遮罩（延时时间，大小，缩放时间）：0.5|0.3|0.3
    /// </summary>
    public void PlayerMaskScale(string value)
    {
    }

    /// <summary>
    /// 战斗UI（延时时间，是否打开）：0.5|True
    /// </summary>
    public void ActiveBattleUI(string value)
    {
    }

    /// <summary>
    /// 创建特效在自身并根据
    /// 延时时间（0.5）|特效路径（Halo/Leizhen）|销毁时间(可选)|是否跟踪(可选)|是否还原父对象（可选）
    /// </summary>
    /// <param name="value"></param>
    public void CreateEffectSelfPosiAndDir(string value)
    {
    }

    //创建特效在自身（延时时间，特效路径）：0.5|Halo/Leizhen|销毁时间(可选)|是否跟踪(可选)|是否跟随大小方向（可选）|是否还原父对象（可选）
    public void CreateEffectSelfPosi(string value)
    {
    }

    /// <summary>
    /// 设置单位到自身（偏移X，偏移Y）：0_0
    /// </summary>
    public void SetUnitToSelfPosi(string value)
    {
    }

    /// <summary>
    /// 设置玩家位置（X，Y）：0_0
    /// </summary>
    public void SetPlayerUnit(string value)
    {
    }

    /// <summary>
    /// 创建光环在自身(延时时间，光环ID, 条件id(BattleSkillCondition, 可选))：0.5|10001|83741
    /// </summary>
    public void CreateHaloSelfPosi(string value)
    {
    }

    //创建特效在相机位置（延时时间，特效路径）：0.5|Halo/Leizhen|销毁时间(可选)
    public void CreateEffectTargetCamera(string value)
    {
    }

    /// <summary>
    /// 跟随相机（跟随X，跟随Y）
    /// </summary>
    public void TargetCamera(string value)
    {
    }

    /// <summary>
    /// 跟随相机（方向：1上2下3左4右）
    /// </summary>
    public void TargetCameraEdge(string value)
    {
    }

    /// <summary>
    /// 延迟激活子物体（多个子物体的父路径，延时时间，激活时间）
    /// </summary>
    public void DelayActiveSubobjectRandomOne(string value)
    {
    }

    /// <summary>
    /// 延迟激活子物体（子物体路径，延时时间，激活时间）
    /// </summary>
    public void DelayActiveSubobject(string value)
    {
    }

    /// <summary>
    /// 延迟激活子物体（子物体路径，延时时间，激活时间）
    /// </summary>
    public void DelayActiveSubobjectUntime(string value)
    {
    }

    private void DelayActiveSubobject(string value, bool isUntime)
    {
    }

    /// <summary>
    /// 销毁自身特效：（延时时间）
    /// </summary>
    public void DestroyEffect(float delay)
    {
    }

    /// <summary>
    /// 循环抖屏：(延时时间，抖屏时间，抖屏等级) 
    /// </summary>
    public void ShakeCameraLoop(string value)
    {
    }

    /// <summary>
    /// 循环抖屏，没有模糊：(延时时间，抖屏时间，抖屏等级) 
    /// </summary>
    public void ShakeCameraLoopNotBlur(string value)
    {
    }

    /// <summary>
    /// 抖屏：(延时时间，抖屏等级) 
    /// </summary>
    public void ShakeCamera(string value)
    {
    }

    /// <summary>
    /// 抖屏：(延时时间，抖动时间，抖动等级) 
    /// </summary>
    public void ShakeRoot(string value)
    {
    }

    /// <summary>
    /// 随机播放动画（动画1、动画2、3456）
    /// </summary>
    public void RandomAnim(string value)
    {
    }

    /// <summary>
    /// 随机间隔播放子节点动画（最短时间，最长时间，随机动画列表）。例：0|1|StateName1|StateName2
    /// </summary>
    public void RandomAnimInterval(string value)
    {
    }

    /// <summary>
    /// 创建特效
    /// </summary>
    private void CreateEffect(string path, Vector2 posi, System.Action<GameObject> returnCall)
    {
    }

    /// <summary>
    /// 破坏周围建筑物（延时时间，破坏半径，是否只破坏带点的）：0|5|False
    /// </summary>
    public void DestroyBarrierDis(string value)
    {
    }

    /// <summary>
    /// 破坏周围建筑物（延时时间，破坏半径，破坏时间）：0|5|0.5
    /// </summary>
    public void BarrierDie(string value)
    {
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

    /// <summary>
    /// 创建残影（延时时间，子物体路径，持续时间，颜色(默认ff0000)，创建间隔(可选)，残影时间（可选），褪色时间（可选））：0|root|1
    /// </summary>
    public void CreateSpriteGhostLoop(string value)
    {
    }

    /// <summary>
    /// 创建角色残影（延时时间，持续时间，颜色(默认ff0000)，创建间隔(可选)，残影时间（可选），褪色时间（可选））：0|root|1
    /// </summary>
    public void CreateSpriteGhostLoopPlayer(string value)
    {
    }

    /// <summary>
    /// 取消跟踪（延时时间|物体路径（可选））：0|5
    /// </summary>
    public void CancelTarget(string value)
    {
    }

    /// <summary>
    /// 延时启用Animator（延时最小时间，延时最大时间）：0.5|0.8
    /// </summary>
    public void DelayActivedAnimator(string value)
    {
    }

    /// <summary>
    /// 修改特效速度（延时时间|速率）：0|2
    /// </summary>
    public void EffectPlayTimeScale(string value)
    {
    }

    /// <summary>
    /// 特殊副本指令
    /// </summary>
    /// <param name="value"></param>
    public void SpecialDungeonFunction(string value)
    {
    }

    /// <summary>
    /// 激活战场（延时时间|禁用时间）：0|2
    /// </summary>
    public void ActiveBattle(string value)
    {
    }

    /// <summary>
    /// 镜头跟随玩家模糊（延时时间|模糊时间|模糊等级|是否所有单位可用模糊（可选））：0|-1|3
    /// </summary>
    public void CameraBlur(string value)
    {
    }

    /// <summary>
    /// 播放音效（延时时间|音效路径|播放次数（默认1））：0|siwang|-1
    /// </summary>
    public void PlaySoundEffect(string value)
    {
    }

    /// <summary>
    /// 播放音效（音效路径1|音效路径2）：siwang1|siwang2
    /// </summary>
    public void RandomPlaySoundEffect(string value)
    {
    }

    /// <summary>
    /// 跟随玩家（位置0脚底，1中心，2头顶）：0
    /// </summary>
    public void TargetPlayer(string value)
    {
    }

    /// <summary>
    ///  设置相机大小和目标 (相机大小, 持续时间, 进入过渡时间, 退出过渡时间)
    /// </summary>
    public void CameraSizeAndTargetSelf(string value)
    {
    }

    /// <summary>
    ///  设置相机大小和目标 (相机大小, 持续时间, 进入过渡时间, 退出过渡时间, 持续时间内锁定目标(goName, 可选))
    /// </summary>
    public void CameraSizeAndTarget(string value)
    {
    }

    /// <summary>
    /// 设置相机大小（相机大小，持续时间，是否影响玩家视野（可选）），缩放时间（可选）：5.4f|999
    /// </summary>
    public void AddOrthoSize(string value)
    {
    }

    private UnitCtrlBase GetUnit(bool isGetPlayer = true)
    {
        UnitCtrlBase unit = null;

        return unit;
    }

    /// <summary>
    /// 延迟激活子物体, 不管理它的生命周期 (子物体路径, 是否开启(开启=1关闭=其他))
    /// </summary>
    public void DelayActiveSubObjectWithoutLifeTimeCtrl(string value)
    {
    }

    /// <summary>
    /// 延迟设置位置 (延迟时间，子物体路径, X, Y)
    /// </summary>
    public void DelaySetLocalPosiSubGo(string value)
    {
    }

    /// <summary>
    /// 激活隐藏障碍物和地板 (延迟时间，是否显示)
    /// </summary>
    public void ActionBarrierAndFloor(string value)
    {
    }


    /// <summary>
    /// 停止背景音乐（延时，背景音量，停用时间）
    /// </summary>
    public void StopBG(string value)
    {
    }
}
