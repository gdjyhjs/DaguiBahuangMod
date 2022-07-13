using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace MOD_FixMod
{
    public class UIMono : MonoBehaviour
    {
        public UIMono(IntPtr ptr) : base(ptr) { }
        public bool open;
        public TaskType taskType = TaskType.Main;
        void OnGUI()
        {
            if (!open)
                return;
            GUI.Button(new Rect(50, 80, 200, 30), "定制修改器");
            if (g.world == null || g.world.playerUnit == null || g.world.playerUnit.data == null || g.world.playerUnit.data.unitData == null || g.world.playerUnit.data.unitData.allTask == null)
            {
                return;
            }
            int y = 150;
            CallQueue cq = new CallQueue();
            List<TaskType> taskTypes = new List<TaskType>() { TaskType.Main, TaskType.School, TaskType.Town, TaskType.Fortuitous };
            List<string> taskTypesName = new List<string>() {"主线","宗门","城镇","奇遇" };
            GUI.Button(new Rect(60, y, 120, 30), "任务类型：");
            int x = 200;
            for (int i = 0; i < taskTypes.Count; i++)
            {
                string name = taskTypes[i] == taskType ? ("·"+ taskTypesName[i]) : taskTypesName[i];
                if (GUI.Button(new Rect(x, y, 50, 30), taskTypesName[i]))
                {
                    taskType = taskTypes[i];
                }
                x += 60;
            }
            y += 60;

            var tasks = g.world.playerUnit.GetTask(taskType);
            foreach (var item in tasks)
            {
                var conf = g.conf.taskBase.GetItem(item.taskData.id);
                GUI.Button(new Rect(80, y, 300, 30), GameTool.LS(conf.name));
                var task = item;
                if (GUI.Button(new Rect(400, y, 80, 30), "强制放弃"))
                {
                    cq.Add(new Action(() =>
                    {
                        UnitActionTaskGive taskGive = new UnitActionTaskGive(task);
                        taskGive.Init(g.world.playerUnit);
                            taskGive.Create();
                    }));
                }
                y += 50;
            }
            cq.RunAllCall();

        }
    }
}