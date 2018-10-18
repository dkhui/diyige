using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;

namespace X.App.Com.Wx
{
    /// <summary>
    /// 多媒体
    /// </summary>
    public class Media : Basic
    {
        public string DownImage(string tk, string mmid)
        {
            return Tools.DownImage("https://api.weixin.qq.com/cgi-bin/media/get?access_token=" + tk + "&media_id=" + mmid);
        }
    }
}
