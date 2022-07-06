using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cave.Patch
{
    public class PatchMgr
    {
        public static PatchMgr instance;

        public bool isTrue = true; // 是否最新版本
        public string trueTips = "请更新MOD";
        public PatchMgr()
        {
            instance = this;

            
        }
    }
}
