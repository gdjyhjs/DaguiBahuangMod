using GuiBaseUI;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Comment
{
    public class CommentMain
    {

        public static string playerUser { get; set; }
        public static string playerName { get; set; }
        public static string playerPassword { get; set; }

        public CommentMain()
        {
            if (PlayerPrefs.HasKey("yellowshange.com.comment.isOpen"))
            {
                UIComment.isOpen = PlayerPrefs.GetInt("yellowshange.com.comment.isOpen") == 1;
            }
            else
            {
                UIComment.isOpen = true;
            }
            Log("八荒点评加载成功！");
        }

        public static void Log(string str)
        {
            Print.Log(str, "[八荒点评]");
        }
    }
}
