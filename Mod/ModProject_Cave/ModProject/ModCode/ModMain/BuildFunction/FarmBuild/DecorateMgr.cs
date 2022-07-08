using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Cave
{
    public class DecorateMgr
    {
        public class DecorateData
        {
            public int id;
            public float x;
            public float y;
            public DecorateData(int id,float x,float y)
            {
                this.id = id;
                this.x = x;
                this.y = y;
            }
        }

        public static string barrierPath = "Battle/ScenesUnit/Barrier/";
        public static Dictionary<int, int> decoratePrice = new Dictionary<int, int>();

        public List<DecorateData> decorateList = new List<DecorateData>();
        public Dictionary<int, GameObject> decorates = new Dictionary<int, GameObject>();
        public Dictionary<GameObject, DecorateData> decorateRecord = new Dictionary<GameObject, DecorateData>();
        public List<Action> operateRecord = new List<Action>();


        public void Init(string decorate)
        {
            try
            {
                decorateList = JsonConvert.DeserializeObject<List<DecorateData>>(decorate);
            }
            catch (Exception)
            {
                decorateList = new List<DecorateData>();
            }
            foreach (var item in decorateList)
            {
                CreateDecorate(item);
            }
        }

        public string GetData()
        {
            return JsonConvert.SerializeObject(decorateList);
        }

        public void CreateDecorate(DecorateData data)
        {
            string path = barrierPath + data.id;
            var prefab = g.res.Load<GameObject>(path);
            if (prefab == null)
                return;

            GameObject obj = Object.Instantiate(prefab);
            Transform objTf = obj.transform;
            obj.name = data.GetHashCode().ToString();
            obj.transform.position = new Vector3(data.x, data.y);
            obj.SetActive(true);

            SpriteRenderer[] sprites = obj.transform.GetComponentsInChildren<SpriteRenderer>();
            Material materialSprite = g.res.Load<Material>("Material/SpriteDefaultGpuInstancing");
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].material = materialSprite;
            }

            BarrierCtrl barrier = obj.GetComponent<BarrierCtrl>();
            ConfBattleBarrierItem barrierConf = g.conf.battleBarrier.GetItem(data.id);
            barrier.InitData(data.id, barrierConf);

            decorates.Add(data.GetHashCode(), obj);
            decorateRecord.Add(obj, data);
        }


        public GameObject CreateDecorate(GameObject prefab, int id)
        {
            GameObject obj = Object.Instantiate(prefab);
            Transform objTf = obj.transform;
            obj.SetActive(true);

            SpriteRenderer[] sprites = obj.transform.GetComponentsInChildren<SpriteRenderer>();
            Material materialSprite = g.res.Load<Material>("Material/SpriteDefaultGpuInstancing");
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].material = materialSprite;
            }

            BarrierCtrl barrier = obj.GetComponent<BarrierCtrl>();
            ConfBattleBarrierItem barrierConf = g.conf.battleBarrier.GetItem(id);
            barrier.InitData(id, barrierConf);
            return obj;
        }

        public void DestroyDecorate(GameObject obj)
        {
            var data = decorateRecord[obj];
            int price = 1000;
            if (decoratePrice.ContainsKey(data.id))
            {
                price = decoratePrice[data.id];
            }
            else
            {
                try
                {
                    Image img = obj.transform.Find("img").GetComponent<Image>();
                    try
                    {
                        price = Mathf.CeilToInt(img.sprite.texture.width * img.sprite.texture.width * 0.1f);
                        decoratePrice.Add(data.id, price);
                    }
                    catch (Exception)
                    {
                        price = 1000;
                    }
                }
                catch (Exception)
                {
                }
            }
            int value = Mathf.FloorToInt(price * 0.8f);
            g.world.playerUnit.data.CostPropItem(PropsIDType.Money, -value);
            SceneType.battle.battleMap.playerUnitCtrl.AddUnitTextTip("灵石 +"+ value);

            decorates.Remove(data.GetHashCode());
            decorateList.Remove(data);
            GameObject.Destroy(obj);
        }


    }
}
