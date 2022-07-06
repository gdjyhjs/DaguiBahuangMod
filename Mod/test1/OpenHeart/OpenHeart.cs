using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Text;
using Il2CppSystem.Threading.Tasks;
using MelonLoader;
using GuiBaseUI;
using UnityEngine;
using UnityEngine.UI;

namespace OpenHeart
{
    // 开局换道心MOD
    public class OpenHeart:MelonMod
    {
        public bool isInit = false;
        public Text ui_heartName;
        public ConfTaoistHeartItem selectHeart;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            Log("打开心扉MOD 欢迎关注我bilibili：八荒大鬼  我的QQ群：50948165", true);
        }

        public void Log(string str, bool show = false)
        {
            if (show)
            {
                MelonLogger.Msg(str);
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            Init();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasUnloaded(buildIndex, sceneName);
            UnInit();
        }

        public void Init()
        {
            if (isInit)
                return;
            try
            {
                Log("监听准备开始！");

                System.Action onOpenCreatePlayer = OnOpenCreatePlayer;
                g.events.On(EGameType.OneOpenUIEnd(UIType.CreatePlayer), onOpenCreatePlayer, -1);

                System.Action onInitCreateGameWorld = OnInitCreateGameWorld;
                g.events.On(EGameType.InitCreateGameWorld, onInitCreateGameWorld, -1);

                System.Action onOpenNpcUI = OnOpenNpcUI;
                g.events.On(EGameType.OneOpenUIEnd(UIType.NPCInfo), onOpenNpcUI, -1);

                System.Action onOpenTown = OnOpenTown;
                g.events.On(EGameType.OneOpenUIEnd(UIType.Town), onOpenTown, -1);

            }
            catch (System.Exception e)
            {
                Log("监听准备失败:" + e.Message);
                return;
            }
            Log( "开局随机道心MOD加载成功！");
            isInit = true;
        }

        public void UnInit()
        {
            if (!isInit)
                return;

            System.Action onOpenCreatePlayer = OnOpenCreatePlayer;
            g.events.Off(EGameType.OneOpenUIEnd(UIType.CreatePlayer), onOpenCreatePlayer);

            System.Action onInitCreateGameWorld = OnInitCreateGameWorld;
            g.events.Off(EGameType.InitCreateGameWorld, onInitCreateGameWorld);

            System.Action onOpenNpcUI = OnOpenNpcUI;
            g.events.Off(EGameType.OneOpenUIEnd(UIType.NPCInfo), onOpenNpcUI);

            System.Action onOpenTown = OnOpenTown;
            g.events.Off(EGameType.OneOpenUIEnd(UIType.Town), onOpenTown);

            isInit = false;
        }

        public void OnOpenCreatePlayer()
        {
            Log( "打开创角界面！");
            var ui = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
            if (ui == null)
                return;

            var ui_bg = CreateUI.NewImage(SpriteTool.GetSprite("NPCInfoCommon", "daoxinmingzikuang"));
            ui_bg.transform.SetParent(ui.property.goGroupRoot.transform, false);
            ui_bg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(-263, 326.5f);
            var ui_name = CreateUI.NewText("");
            ui_heartName = ui_name.GetComponent<UnityEngine.UI.Text>();
            ui_heartName.color = UnityEngine.Color.black;
            ui_heartName.alignment = TextAnchor.MiddleCenter;
            ui_name.transform.SetParent(ui_bg.transform, false);
            System.Action onClick = RandomHeart;
            ui.property.btnPropertyRandom.onClick.AddListener(onClick);
            ui.property.btnPropertyRandom_En.onClick.AddListener(onClick);
            ui_heartName.gameObject.AddComponent<UISkyTipEffect>();
            RandomHeart();

        }

        public void RandomHeart()
        {
            var ui = g.ui.GetUI<UICreatePlayer>(UIType.CreatePlayer);
            if (ui == null)
                return;

            List<ConfBaseItem> list = g.conf.taoistHeart.allConfBase;
            int idx = CommonTool.Random(0, list.Count);
            selectHeart = g.conf.taoistHeart.GetItem(list[idx].id);
            ui_heartName.text = GameTool.LS(selectHeart.heartName);
            ui_heartName.gameObject.GetComponent<UISkyTipEffect>().InitData(GetHeartDes());
        }

        public void OnInitCreateGameWorld()
        {
            if (selectHeart != null)
            {
                WorldUnitBase unit = g.world.playerUnit;
                unit.data.unitData.heart.InitHeart(unit, selectHeart);
            }
        }

        public string GetHeartDes()
        {
            List<string> list = new List<string>();
            list.Add("道心：" + GameTool.LS(selectHeart.heartName));

            ConfTaoistHeartEffectItem heartEffectItem = g.conf.taoistHeartEffect.GetItemInLevel(selectHeart.id, 1);
            if (heartEffectItem.feature != 0)
            {
                list.Add(UIMartialInfoTool.GetDescRichText("\n道心初生：\n"+GameTool.LS(g.conf.roleCreateFeature.GetItem(heartEffectItem.feature).tips), new BattleSkillValueData(1), 1));
            }
            heartEffectItem = g.conf.taoistHeartEffect.GetItemInLevel(selectHeart.id, 2);
            if (heartEffectItem.feature != 0)
            {
                list.Add(UIMartialInfoTool.GetDescRichText("\n道心凝华：\n" + GameTool.LS(g.conf.roleCreateFeature.GetItem(heartEffectItem.feature).tips), new BattleSkillValueData(1), 1));
            }
            heartEffectItem = g.conf.taoistHeartEffect.GetItemInLevel(selectHeart.id, 3);
            if (heartEffectItem.feature != 0)
            {
                list.Add(UIMartialInfoTool.GetDescRichText("\n道心结粹：\n" + GameTool.LS(g.conf.roleCreateFeature.GetItem(heartEffectItem.feature).tips), new BattleSkillValueData(1), 1));
            }

            List<ConfFateFeatureItem> fateList = GetFate(selectHeart);
            StringBuilder strb = new StringBuilder();
            strb.Append("\n"+GameTool.LS("upGrade_nitiangaimingtishi") + "\n\n");
            for (int i = 0; i < fateList.Count; i++)
            {
                ConfRoleCreateFeatureItem createFeatureItem = g.conf.roleCreateFeature.GetItem(fateList[i].id);
                string name = GameTool.LS(createFeatureItem.name);
                if (createFeatureItem.type == 3 || createFeatureItem.level < 0)
                {
                    name = CommonTool.SetStrColor(name, GameTool.LevelToColor(Mathf.Abs(createFeatureItem.level), 2));
                }

                strb.Append(name);

                if (i < fateList.Count - 1)
                {
                    strb.Append("、");
                }
            }
            list.Add(strb.ToString());
            string result = string.Join("\n", list.ToArray());
            Log("输入内容：" + result);
            return result;
        }

        public List<ConfFateFeatureItem> GetFate(ConfTaoistHeartItem heartConf)
        {
            Log("获取可能出现的逆天改命");
            List<int> heartGroup = new List<int>();
            if (heartConf.fateFeatureGroupID != 0) {
                List<ConfBaseItem> allGroup = g.conf.fateFeatureGroup.allConfBase;
                foreach (var item in allGroup)
                {
                    ConfFateFeatureGroupItem a = g.conf.fateFeatureGroup.GetItem(item.id);
                    if (a.groupID == heartConf.fateFeatureGroupID)
                    {
                        heartGroup.Add(a.fateFeatureID);
                    }
                }
            }
            Log("计算完了逆天改命组");
            List<ConfBaseItem> allFate = g.conf.fateFeature.allConfBase;
            List<ConfFateFeatureItem> items = new List<ConfFateFeatureItem>();

            if (heartConf.heartSkills.Count > 0 && heartConf.heartSkills[0] > 0)
            {
                var conf = g.conf.fateFeature.GetItem(heartConf.heartSkills[0]);
                items.Add(conf);
            }
            Log("加入了特殊逆天改命");
            foreach (var item in allFate)
            {
                if (heartConf.fateFeatureGroupID != 0 && !heartGroup.Contains(item.id))
                    continue;
                var conf = g.conf.fateFeature.GetItem(item.id);
                if (conf.weightBase == 0)
                    continue;
                items.Add(conf);
            }
            Log("统计完毕");
            return items;
        }

        public void OnOpenNpcUI()
        {
            var ui = g.ui.GetUI<UINPCInfo>(UIType.NPCInfo);
            if (ui == null)
                return;
            GameObject go = GameObject.Instantiate(ui.unitInfo.goButton1);
            go.transform.SetParent(ui.unitInfo.goGroupRoot.transform, false);
            go.transform.SetAsFirstSibling();
            go.GetComponentInChildren<Text>().text = "求道";

            System.Action action = () =>
            {
                if (g.world.playerUnit.data.unitData.heart.state != DataUnit.TaoistHeart.HeartState.Empty)
                {
                    OpenDrama(ui.unit, "你已经有自己的道了，还来戏弄我，是不是找死啊？");
                    return;
                }
                if (ui.unit.data.unitData.heart.state != DataUnit.TaoistHeart.HeartState.Complete)
                {
                    OpenDrama(ui.unit, "小孩子一边玩去，我自己的道都还没摸清楚呢！");
                    return;
                }
                int intim = Mathf.RoundToInt(ui.unit.data.unitData.relationData.intimToPlayerUnit);
                if (intim < 10)
                {
                    OpenDrama(ui.unit, "我跟你很熟吗？");
                    return;
                }
                UnitActionSetTaoistHeart roleRelation = new UnitActionSetTaoistHeart(g.world.playerUnit, ui.unit.data.unitData.heart.HeartConf());
                ui.unit.CreateAction(roleRelation);
            };
            go.GetComponentInChildren<Button>().onClick.AddListener(action);
            go.SetActive(true);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250);
        }

        public void OnOpenTown()
        {
            var ui = g.ui.GetUI<UITown>(UIType.Town);
            if (ui == null)
                return;


            var ui_bg = CreateUI.NewImage(SpriteTool.GetSprite("TownCommon", "Build2008"));
            ui_bg.transform.SetParent(ui.imgBG.transform, false);
            ui_bg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(-371.1f, -239.5f);
            System.Action action = () =>
            {
                int[] monenys = new int[] {500, 2000, 8000, 20000, 50000, 80000, 20000, 30000, 50000, 100000 };
                int grade = g.world.playerUnit.data.dynUnitData.GetGrade();
                if (grade < 0 || grade >= monenys.Length)
                {
                    grade = monenys.Length - 1;
                }
                int needMoney = monenys[grade];
                System.Action action2 = () =>
                {
                    int money = g.world.playerUnit.data.unitData.propData.GetPropsNum((int)PropsIDType.Money);
                    if (money < needMoney)
                    {
                        OpenDrama2("小子，没灵石想来找死吗？");
                    }
                    else if (g.world.playerUnit.data.unitData.heart.state != DataUnit.TaoistHeart.HeartState.Complete)
                    {
                        OpenDrama2("无道心者，没资格换心。");
                    }
                    else
                    {
                        if (CommonTool.Random(0f, 100f) < 10)
                        {
                            g.world.playerUnit.data.unitData.heart.AddHeartEffectHP(g.world.playerUnit, g.world.playerUnit, -1000000);
                            List<GameItemRewardData> rewards = new List<GameItemRewardData>();
                            rewards.Add(new GameItemRewardData(1, new int[] { PropsIDType.Money }, needMoney));
                            g.world.playerUnit.data.RewardItem(rewards);
                            OpenDrama2("很抱歉，道心没换成功，这些灵石算是对你的补偿！");
                        }
                        else
                        {
                            List<ConfBaseItem> list = g.conf.taoistHeart.allConfBase;
                            int idx = CommonTool.Random(0, list.Count);
                            var heart = g.conf.taoistHeart.GetItem(list[idx].id);
                            WorldUnitBase unit = g.world.playerUnit;
                            unit.data.unitData.heart.InitHeart(unit, heart);
                            g.world.playerUnit.data.CostPropItem(PropsIDType.Money, needMoney);
                            OpenDrama2($"道心帮你换了{GameTool.LS(heart.heartName)}，快去享受吧！");
                        }
                    }
                };
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), $"以你当前境界需要换心的话，需要花费{needMoney}灵石，我可以随机帮你换一颗道心，是否继续？", 2, action2);

            };
            ui_bg.AddComponent<Button>().onClick.AddListener(action);

            var ui_namebg = CreateUI.NewImage(SpriteTool.GetSprite("Common", "jianzhumingzibg"));
            ui_namebg.transform.SetParent(ui_bg.transform, false);
            ui_namebg.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(-65.8f, 10.2f);
            ui_namebg.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(30, 101);

            var ui_name = CreateUI.NewText("摘心阁");
            ui_name.transform.SetParent(ui_bg.transform, false);
            ui_name.GetComponent<UnityEngine.RectTransform>().anchoredPosition = new UnityEngine.Vector2(-64f, 10.2f);
            ui_name.GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(25, 101);
            ui_name.GetComponent<UnityEngine.UI.Text>().lineSpacing = 0.65f;
        }

        public void OpenDrama(WorldUnitBase unit, string str = "")
        {
            UICustomDramaDyn dramaDyn = new UICustomDramaDyn(21802);
            dramaDyn.dramaData.unit = unit;
            dramaDyn.dramaData.dialogueText[21802] = str;
            dramaDyn.OpenUI();
        }

        // 神秘人说话
        public void OpenDrama2(string str = "")
        {
            g.conf.dramaDialogue.GetItem(1009037).nextDialogue = "0";
            UICustomDramaDyn dramaDyn = new UICustomDramaDyn(1009037);
            dramaDyn.dramaData.dialogueText[1009037] = str;
            dramaDyn.OpenUI();
        }
    }
}
