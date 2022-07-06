using Cave.Config;
using Cave.Patch;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Cave.BuildFunction
{
    // 客房
    public class BuildGuest : GuiBaseUI.ClassBase
    {
        public BuildGuest()
        {
        }

        public override void Init(string param)
        {
            if (g.ui.GetUI(UIType.NPCSearch) != null)
                return;
            Cave.Log("打开厢房");
            DataCave data = DataCave.ReadData();
            Patch_UINPCUnitInfoItem_InitData.addOpenChangeFace = true;
            UINPCSearch ui = g.ui.OpenUI<UINPCSearch>(UIType.NPCSearch);
            var keys = new List<string>(data.npcDatas.Keys);
            Il2CppSystem.Collections.Generic.List<string> allunit = new Il2CppSystem.Collections.Generic.List<string>(keys.Count);
            foreach (var item in keys)
            {
                allunit.Add(item);
            }
            ui.units = g.world.unit.GetUnit(allunit);
            ui.items.Clear();
            ui.OnUpdateList();

            var parent = ui.transform;
            var go =GuiBaseUI.CreateUI.NewText(ConfBuild.GetItem(4004).GetName());
            go.name = "openChangeFace";
            go.transform.SetParent(parent, false);
            go.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 50);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-550, -425);
            go.GetComponent<Text>().color = Color.black;


        }
    }
}
