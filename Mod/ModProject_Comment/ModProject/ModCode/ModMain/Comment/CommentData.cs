using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comment
{
    public class CommentItem
    {
        public string a; // 内容
        public int b; // 时间
        public int c; // 好评
        public int d; // 差评
        public string e; // 昵称
        public int f; // 游戏名
        public int g; // 类型
        public int h; // 对象
        public string connect { get { return a; } }
        public int time { get { return b; } }
        public int good { get { return c; } }
        public int bad { get { return d; } }
        public string name { get { return e; } }
        public int game { get { return f; } }
        public int type { get { return g; } }
        public int target { get { return h; } }
    }

    public class CommentData
    {
        public List<CommentItem> items;
        public int updateTime;
    }

    public class LoginData
    {
        public string a; // 账号
        public string b; // 名字
        public string c; // IP地址
        public string user { get { return a; } }
        public string name { get { return b; } }
        public string ip { get { return c; } }

    }
}
