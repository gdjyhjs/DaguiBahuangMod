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
        public static List<int> ignoreBarrierID = new List<int>()
        {
            10401,10402,10501,10502,10503,17801,10901,11001,11301,11501,11601,11701,20201,20301,20401,20501,20801,20802,20901,21701,21801,22101,22601,22701,22801,
            23001,30601,30701,30801,30901,31301,31401,31601,31701,31801,32101,32201,32401,32501,32701,32901,33001,33401,140504,141301,440101,440301,450001,467100,
            469100,470101,480101,490101,500801,520101,530101,420101,420201,430101,411301,330001,380101,350101,340101,400101,360501,360502,360503,360504,360505,240501,
            250101,260101,270101,270201,280101,280201,280301,280401,290101,290201,290301,290401,300101,300201,300301,300401,300501,300601,300701,300801,300901,301001,
            301101,301201,310101,310201,310301,310401,310501,310601,360101,360201,360301,360302,360401,360402,360403,360404,360405,232501,232401,240101,240201,240301,
            240401,200001,110101,120101,80401,17701,140501,140601,140701,190201,182101,211601,211602,212001,212002,510019,160101
        };

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
            try
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
            catch (Exception e)
            {
                Debug.LogWarning("创建失败 id="+id);
                Debug.LogWarning(e.Message + "\n" + e.StackTrace);
                return new GameObject();
            }
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
