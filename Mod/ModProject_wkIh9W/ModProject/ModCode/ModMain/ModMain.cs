using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using MOD_wkIh9W.Item;

/// <summary>
/// 当你手动修改了此命名空间，需要去模组编辑器修改对应的新命名空间，程序集也需要修改命名空间，否则DLL将加载失败！！！
/// </summary>
namespace MOD_wkIh9W
{
	/// <summary>
	/// 此类是模组的主类
	/// </summary>
	public class ModMain
	{


		private static HarmonyLib.Harmony harmony;

		/// <summary>
		/// MOD唯一ID
		/// </summary>
		public const string MOD_SOLE_ID = "wkIh9W";

        public static ConfDaguiToolBase confTool;

        public static DataStruct<TimeScaleType, DataStruct<float>> changeSpeed;
        public static float gameSpeed = 1;
        public static DataStruct<WorldUnitBase, DataProps.MartialData> lastAddMartial { get; set; }
        public static DataStruct<WorldUnitBase, DataUnit.ActionMartialData> lastStudySkill { get; set; }
        public static DataStruct<WorldUnitBase, DataProps.PropsData> lastAddProp { get; set; }
        public static DataStruct<WorldUnitBase, DataProps.PropsData> lastAddElder { get; set; }
        public static DataStruct<WorldUnitBase, DataProps.PropsData> lastAddRule { get; set; }
        public static DataStruct<WorldUnitBase, DataProps.PropsData> lastAddArtifact { get; set; }
        public static DataStruct<WorldUnitBase, DataUnit.ActionMartialData> lastStudyAbility { get; set; }
        public static WorldUnitBase lastUnit;
        public static int chinaInit = 0;
        public static float scrollSpeed = 100;

        public static string dataPath
        {
            get
            {
                return g.cache.cachePath + "/DaguiToolCmdData.cache";
            }
        }

        public static string newDataPath
        {
            get
            {
                return g.cache.cachePath + "/../DaguiToolCmdData.cache";
            }
        }

        public static List<CmdItem> allCmdItems { get { return cmdData.allCmdItems; } }

        public static CmdData cmdData { get
            {
                if (m_cmdData == null)
                {
                    Action InitData = () =>
                    {
                        m_cmdData = new CmdData();
                        m_cmdData.key.key = KeyCode.F2.ToString();
                    };


                    try
                    {
                        if (!File.Exists(newDataPath))
                        {
                            if (File.Exists(dataPath))
                            {
                                File.Copy(dataPath, newDataPath, true);
                                Console.WriteLine("复制旧数据成功");
                                chinaInit = 1;
                            }
                            else
                            {
                                string filePath = g.mod.GetModPathRoot("wkIh9W") + "/ModAssets/DaguiToolCmdData.cache";
                                File.Copy(filePath, newDataPath, true);
                                Console.WriteLine("复制预设数据成功");
                                chinaInit = 1;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        if (File.Exists(newDataPath))
                        {
                            string str = File.ReadAllText(newDataPath);
                            Console.WriteLine("保存数据\n" + str);
                            m_cmdData = JsonConvert.DeserializeObject<CmdData>(str);
                            if (m_cmdData == null)
                            {
                                InitData();
                            }
                        }
                        else
                        {
                            InitData();
                        }
                    }
                    catch (Exception)
                    {
                        InitData();
                    }
                    Console.WriteLine("初始化大鬼工具盒配置");
                    if (g.res.Load<TextAsset>("ModConf/DafuiToolBase") != null)
                    {
                        //获取配置
                        confTool = new ConfDaguiToolBase(g.res.Load<TextAsset>("ModConf/DafuiToolBase").text);
                    }
                }
               return m_cmdData;
            }
        }

        private static CmdData m_cmdData;

        public static void SaveCmdItems()
        {
            string str = JsonConvert.SerializeObject(cmdData);
            Console.WriteLine("保存数据\n"+str);
            File.WriteAllText(newDataPath, str);
        }

        public static CmdRun cmdRun;


        private Il2CppSystem.Action<ETypeData> onOnInitWorldCall;
		private Il2CppSystem.Action<ETypeData> onOpenUICall;

		/// <summary>
		/// MOD初始化，进入游戏时会调用此函数
		/// </summary>
		public void Init()
        {
            try
            {
                //使用了Harmony补丁功能的，需要手动启用补丁。
                //启动当前程序集的所有补丁
                if (harmony != null)
                {
                    harmony.UnpatchSelf();
                    harmony = null;
                }
                if (harmony == null)
                {
                    harmony = new HarmonyLib.Harmony("MOD_wkIh9W");
                }
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                onOnInitWorldCall = (Il2CppSystem.Action<ETypeData>)OnIntoWorld;
                onOpenUICall = (Il2CppSystem.Action<ETypeData>)OnOpenUI;

                g.events.On(EGameType.IntoWorld, onOnInitWorldCall);
                g.events.On(EGameType.OpenUIEnd, onOpenUICall);
                Console.WriteLine("初始化大鬼工具盒ok");
            }
            catch (Exception e)
            {
                Console.WriteLine("初始化大鬼工具盒f " + e.ToString());
            }

            try
            {
                string toDir = "MelonLoader/Managed/";
                if (!File.Exists(toDir + "NStandard.dll"))
                {
                    string dllPath = g.mod.GetModPathRoot("wkIh9W") + "/ModCode/dll/NStandard.dll";
                    File.Copy(dllPath, toDir + "NStandard.dll", true);
                    Console.WriteLine("复制NStandard.dll成功");
                    chinaInit=1;
                }
                else
                {
                    Console.WriteLine("已存在NStandard.dll");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("复制NStandard.dll失败 " + e.ToString());
                chinaInit=2;
            }

            if (cmdRun == null)
            {
                GameObject go = new GameObject("DaguiCmdRun");
                ClassInjector.RegisterTypeInIl2Cpp<CmdRun>();
                cmdRun = go.AddComponent<CmdRun>();
                GameObject.DontDestroyOnLoad(go);
            }
        }

		/// <summary>
		/// MOD销毁，回到主界面，会调用此函数并重新初始化MOD
		/// </summary>
		public void Destroy()
		{

        }

        /// <summary>
        /// 进入世界回调
        /// </summary>
        private void OnIntoWorld(ETypeData e)
        {
            Console.WriteLine("进入世界 初始化工具盒");

            // 初始化数据
            UISelectChar.findItems = null;
            UISelectChar.allItems = null;
        }

        /// <summary>
        /// 每帧调用的函数
        /// </summary>
        public static void OnUpdate()
        {
            if (cmdData.key.IsKeyDown())
            {
                UIBase ui = g.ui.GetUI(new UIType.UITypeBase("DaguiTool", UILayer.UI));
                if (ui == null)
                {
                    OpenUI<UIDaguiTool>("DaguiTool").InitData();
                }
                else
                {
                    while (g.ui.GetLayerTopUI(UILayer.UI) != ui)
                    {
                        g.ui.CloseUI(g.ui.GetLayerTopUI(UILayer.UI));
                    }
                    g.ui.CloseUI(ui);
                }
            }

            if (changeSpeed != null && GameTool.timeScales != null && !GameTool.timeScales.Contains(changeSpeed))
            {
                UpdateSpeed();
            }
        }
        /// <summary>
        /// 注入UI到大地图的主界面上
        /// </summary>
        private void OnOpenUI(ETypeData e)
        {
            EGameTypeData.OpenUIEnd edata = e.Cast<EGameTypeData.OpenUIEnd>();

            if (edata.uiType.uiName == UIType.MapMain.uiName)
            {
                MapMainAddBtn();
            }
        }

        /// <summary>
        /// 注入UI到大地图
        /// </summary>
        private void MapMainAddBtn()
        {
            UIMapMain uiMapMain = g.ui.GetUI<UIMapMain>(UIType.MapMain);
            if (uiMapMain != null && uiMapMain.playerInfo.btnPlayer.transform.parent.Find("BtnOpenDaguiTool") == null)
            {
                GameObject goBtn = GameObject.Instantiate(g.res.Load<GameObject>("UI/BtnOpenDaguiTool"), uiMapMain.playerInfo.btnPlayer.transform.parent);
                goBtn.name = "BtnOpenDaguiTool";
                goBtn.transform.localPosition = new Vector3(40, 420);
                goBtn.GetComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    OpenUI<UIDaguiTool>("DaguiTool").InitData();
                }));

                goBtn.gameObject.AddComponent<UISkyTipEffect>().InitData("大鬼工具盒");
            }
        }

        public static T OpenUI<T>(string uiName) where T : MonoBehaviour
        {
            ClassInjector.RegisterTypeInIl2Cpp<T>();
            UIBase uiModTransf = g.ui.OpenUI(new UIType.UITypeBase(uiName, UILayer.UI));
            return uiModTransf.gameObject.AddComponent<T>();
        }

        public static void FixGameSpeed(float speed)
        {
            gameSpeed = speed; 
            UpdateSpeed();
        }

        private static void UpdateSpeed()
        {
            if (Mathf.Approximately(gameSpeed, 1.0f))
            {
                changeSpeed = null;
                GameTool.ResetTimeScale();
            }
            else
            {
                if (changeSpeed != null && GameTool.timeScales.Contains(changeSpeed))
                {
                    GameTool.timeScales.Remove(changeSpeed);
                }
                changeSpeed = new DataStruct<TimeScaleType, DataStruct<float>>(TimeScaleType.SlowTime, new DataStruct<float>(gameSpeed));
                GameTool.timeScales.Add(changeSpeed);
                GameTool.SetTimeScale(GameTool.GetMinTimeScale());
            }
        }
    }
}
