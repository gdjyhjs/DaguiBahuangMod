using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Comment
{
    public class CommentMain : MelonMod
    {

        public static string playerUser { get; set; }
        public static string playerName { get; set; }
        public static string playerPassword { get; set; }

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            if (PlayerPrefs.HasKey("yellowshange.com.comment.isOpen"))
            {
                UIComment.isOpen = PlayerPrefs.GetInt("yellowshange.com.comment.isOpen") == 1;
            }
            else
            {
                UIComment.isOpen = true;
            }
        }
    }
}
