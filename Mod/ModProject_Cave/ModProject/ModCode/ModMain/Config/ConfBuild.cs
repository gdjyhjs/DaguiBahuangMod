using Cave.BuildFunction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cave.Config
{
    public class ConfBuildItem
    {
        public int id; // 配置ID
        public string condition; // 使用条件
        public string function; // 条件判定通过的建筑功能
        public string failfuntion; // 条件判定不通过的建筑功能
        public string enterfuntion = ""; // 鼠标进入功能
        public string exitfuntion = ""; // 鼠标滑出功能
        public string createfuntion = ""; // 创建建筑功能
        public int type; // 类型
        public int price; // 价格
        public int stone; // 购买需要的石头
        public int wood; // 购买需要的木柴
        public bool hideName; // 是否显示名字
        public string name; // 名字
        public string atlas; // 图集
        public string image; // 图片
        public string[] effect; // 特效
        public int maxLevel; // 最大等级
        public int maxCount; // 最大持有数量
        public int[] upgradeMoney; // 升级金钱
        public int[] upgradeStone; // 升级需要的石头
        public int[] upgradeWood; // 升级需要的木柴
        public string des = ""; // 建筑描述

        public bool CheckCost(int checkNeedPrice = -1, int checkNeedStone = -1, int checkNeedWood = -1)
        {
            if (checkNeedPrice == -1)
            {
                checkNeedPrice = this.price;
            }
            if (checkNeedStone == -1)
            {
                checkNeedStone = this.stone;
            }
            if (checkNeedWood == -1)
            {
                checkNeedWood = this.wood;
            }
            int money = g.world.playerUnit.data.unitData.propData.GetPropsNum(PropsIDType.Money);
            int stone = g.world.playerUnit.data.unitData.propData.GetPropsNum(MountainBuild.stoneId);
            int wood = g.world.playerUnit.data.unitData.propData.GetPropsNum(MountainBuild.woodId);
            if (money < checkNeedPrice)
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("common_lingshibuzu"), 1);
            }
            else if (stone < checkNeedStone)
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_NeedStone"), 1);
            }
            else if (wood < checkNeedWood)
            {
                g.ui.OpenUI<UICheckPopup>(UIType.CheckPopup).InitData(GameTool.LS("common_tishi"), GameTool.LS("Cave_NeedWood"), 1);
            }
            else
            {
                return true;
            }
            return false;
        }
        public void BuyCost()
        {
            g.world.playerUnit.data.CostPropItem(PropsIDType.Money, price);
            g.world.playerUnit.data.CostPropItem(MountainBuild.stoneId, stone);
            g.world.playerUnit.data.CostPropItem(MountainBuild.woodId, wood);
        }
        public void UpGradeCost(int price, int stone, int wood)
        {
            g.world.playerUnit.data.CostPropItem(PropsIDType.Money, price);
            g.world.playerUnit.data.CostPropItem(MountainBuild.stoneId, stone);
            g.world.playerUnit.data.CostPropItem(MountainBuild.woodId, wood);
        }
        public string GetSpriteDesc(int checkNeedPrice = -1, int checkNeedStone = -1, int checkNeedWood = -1)
        {
            if (checkNeedPrice == -1)
            {
                checkNeedPrice = this.price;
            }
            if (checkNeedStone == -1)
            {
                checkNeedStone = this.stone;
            }
            if (checkNeedWood == -1)
            {
                checkNeedWood = this.wood;
            }
            string descStr = $"";
            int priceTypeCount = 0;
            if (checkNeedPrice > 0)
            {
                descStr += $"{checkNeedPrice}{GameTool.LS("Cave_Lingshi")}";
                priceTypeCount++;
            }
            if (checkNeedStone > 0)
            {
                string flag = priceTypeCount == 0 ? "" : "、";
                descStr += $"{flag}{checkNeedStone}{GameTool.LS("Cave_Stone")}";
                priceTypeCount++;
            }
            if (checkNeedWood > 0)
            {
                string flag = priceTypeCount == 0 ? "" : "、";
                descStr += $"{flag}{checkNeedWood}{GameTool.LS("Cave_Wood")}";
            }
            return descStr;
        }

        public string GetName()
        {
            return GameTool.LS(ConfBuild.GetItem(id).name);
        }

        public Sprite GetSprite()
        {
            var item = ConfBuild.GetItem(id);
            if (File.Exists("Mods/Cave/" + id + ".png"))
            {
                var config = GetConfig();
                if (config != null)
                {
                    return CaveTool.LoadSprite("Mods/Cave/" + id + ".png", config.width, config.height);
                }
                else
                {
                    return CaveTool.LoadSprite("Mods/Cave/" + id + ".png");
                }
            }

            if (!string.IsNullOrWhiteSpace(item.atlas))
            {
                var sprite = SpriteTool.GetSprite(item.atlas, item.image);
                if (sprite != null)
                {
                    return sprite;
                }
            }
            else if (!string.IsNullOrWhiteSpace(item.image))
            {
                var sprite = SpriteTool.GetSpriteBigTex(item.image);
                if (sprite != null)
                {
                    return sprite;
                }
            }
            else if (item.type == 1)
            {
                var sprite = SpriteTool.GetSprite("TownCommon", "Build" + id);
                if (sprite != null)
                {
                    return sprite;
                }
            }
            else if (item.type == 2)
            {
                var sprite = SpriteTool.GetSprite("SchoolCommon", "BuildUp" + id);
                if (sprite == null)
                {
                    sprite = SpriteTool.GetSprite("SchoolCommon", "BuildDown" + id);
                }
                int lv = 1;
                if (sprite == null)
                {
                    sprite = SpriteTool.GetSprite("SchoolCommon", "BuildUp" + id + "_" + lv);
                }
                if (sprite == null)
                {
                    sprite = SpriteTool.GetSprite("SchoolCommon", "BuildDown" + id + "_" + lv);
                }
                if (sprite != null)
                {
                    return sprite;
                }
            }
            return SpriteTool.GetSprite("TownCommon", "Build2007");
        }

        public BuildConfig GetConfig()
        {
            if (Cave.config.buildConfig.ContainsKey(id))
            {
                return Cave.config.buildConfig[id];
            }
            else
            {
                return null;
            }
        }
    }

    public static class ConfBuild
    {
        public static string dir;
        public static string path = "*CaveBuild.json";
        public static List<ConfBuildItem> list = new List<ConfBuildItem>();
        public static Dictionary<int, ConfBuildItem> items = new Dictionary<int, ConfBuildItem>();
        static ConfBuild()
        {
            dir = Cave.GetModDir();
            Init();
        }

        public static void Init()
        {
            if (!Directory.Exists(dir))
            {
                Cave.Log("无法找到配置。" + dir, 0);
                Cave.Log("请检查Cave文件夹是否正常。" + dir, 0);
                return;
            }
            list.Clear();
            DirectoryInfo direction = new DirectoryInfo(dir);
            FileInfo[] files = direction.GetFiles(path, SearchOption.AllDirectories);
            //Cave.Log("读取建筑配置数量" + files.Length + " 如果有异常请检查Cave文件夹里的配置文件", 0);
            for (int i = 0; i < files.Length; i++)
            {
                var item = files[i];
                try
                {
                    //Cave.Log("读取配置文件：" + item.FullName);
                    List<ConfBuildItem> obj = JsonConvert.DeserializeObject<List<ConfBuildItem>>(File.ReadAllText(item.FullName));
                    foreach (var objItem in obj)
                    {
                        list.Add(objItem);
                    }
                }
                catch (Exception e)
                {
                    Cave.Log("配置文件错误。" + item.FullName + "\n" + e.Message, 0);
                }
            }

            items.Clear();
            foreach (ConfBuildItem item in list)
            {
                if (items.ContainsKey(item.id))
                {
                    items[item.id] = item;
                    //Cave.Log($"修正配置 id={item.id} function={item.function} type={item.type} price={item.price} name={item.name} maxLevel={item.maxLevel}  ", 2);
                }
                else
                {
                    items.Add(item.id, item);
                    //Cave.Log($"增加配置 id={item.id} function={item.function} type={item.type} price={item.price} name={item.name} maxLevel={item.maxLevel}  ", 2);
                }
            }
        }

        public static ConfBuildItem GetItem(int id)
        {
            if (items.ContainsKey(id))
            {
                return items[id];
            }
            else
            {
                return null;
            }
        }

    }
}
