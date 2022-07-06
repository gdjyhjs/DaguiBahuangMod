using GuiBaseUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cave.BuildFunction
{
    public class FaZhenData {
        public bool open = false;

        // 是否开启阵法
        public static bool IsOpen(DataCave dataCave = null)
        {
            if (dataCave == null)
            {
                dataCave = DataCave.ReadData();
            }
            CaveBuildData item = dataCave.GetBuild(3002);
            if (item != null)
            {
                FaZhenData data = string.IsNullOrWhiteSpace(item.param) ? new FaZhenData() : JsonConvert.DeserializeObject<FaZhenData>(item.param);
                return item.put && data.open;
            }
            return false;
        }
    }

    public class CreateBuildFazhen : GuiBaseUI.ClassBase
    {


        public CreateBuildFazhen()
        {
        }

        public override void Init(string param)
        {
            var item = MainCave.createItem;
            var go = MainCave.createItemObj;
            FaZhenData data = string.IsNullOrWhiteSpace(item.param) ? new FaZhenData() : JsonConvert.DeserializeObject<FaZhenData>(item.param);
            if (data.open)
            {
                var effectRoot = CreateUI.New();
                effectRoot.transform.SetParent(go.transform, false);
                effectRoot.GetComponent<RectTransform>().sizeDelta = go.GetComponent<RectTransform>().sizeDelta;
                var effectGo = Cave.CreateGo("Effect/UI/chuansongtishi2", effectRoot.transform, effectRoot.GetComponentInParent<Canvas>().sortingOrder + 1);
                GameEffectTool.SetEffectOutsideMask(effectGo);
                effectRoot.transform.localScale = Vector3.one * 5;
                effectRoot.transform.SetSiblingIndex(1);
            }
        }
    }
    public class BuildBuildFazhen : GuiBaseUI.ClassBase
    {
        public BuildBuildFazhen()
        {
        }

        public override void Init(string param)
        {
            var item = MainCave.data.GetBuild(3002);
            FaZhenData data = string.IsNullOrWhiteSpace(item.param) ? new FaZhenData() : JsonConvert.DeserializeObject<FaZhenData>(item.param);
            data.open = !data.open;
            if (data.open)
            {
                UITipItem.AddTip("开启了洞府法阵！");
            }
            else
            {
                UITipItem.AddTip("关闭了洞府法阵！");
            }
            item.param = JsonConvert.SerializeObject(data);
            DataCave.SaveData(MainCave.data);
            MainCave.data.InitCave();
            DramaFunction.UpdateMapAllUI();
            MainCave.cave.CreateBuilds();
        }
    }
}
