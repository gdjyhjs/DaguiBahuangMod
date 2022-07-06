using MelonLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Gymnasium
{
    public class Class1 : MelonMod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKeyDown(KeyCode.U))
            {
                new Gumnasium().Init("");
            }
        }
    }
}
