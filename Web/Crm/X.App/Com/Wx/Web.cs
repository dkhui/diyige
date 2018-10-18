using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Core.Plugin;
using X.Core.Utility;

namespace X.App.Com.Wx
{
    /// <summary>
    /// 网页开发
    /// </summary>
    public class Web
    {
        public token GetToken(string code)
        {
            var json = Tools.GetHttpData("https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + appsecret + "&code=" + code + "&grant_type=authorization_code");
            var token = Serialize.FromJson<token>(json);
            if (!string.IsNullOrEmpty(token.errcode)) { Loger.Error(json); }
            return token;
        }

        public class token : Basic.mbase
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string openid { get; set; }
            public string scope { get; set; }
        }

        public string GetJsTicket(string tk)
        {
            return GetJsTicket(tk, false);
        }

        public string GetJsTicket(string tk, bool isnew)
        {
            var tick = CacheHelper.Get<string>("wx.js_ticket");
            if (string.IsNullOrEmpty(tick) || isnew)
            {
                var json = Tools.GetHttpData("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + tk + "&type=jsapi");
                var tkn = Serialize.FromJson<tick>(json);
                tick = tkn.ticket;
                CacheHelper.Save("wx.js_ticket", tick, tkn.expires_in - 500);
            }
            return tick;
        }

        string RefreshToken()
        {
            var re_tk = CacheHelper.Get<string>("wx.refresh_token");
            if (string.IsNullOrEmpty(re_tk)) return "";
            var json = Tools.GetHttpData("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=" + appid + "&grant_type=refresh_token&refresh_token=" + re_tk);
            var rtk = Serialize.FromJson<token>(json);
            if (!string.IsNullOrEmpty(rtk.errcode)) Loger.Error(json);
            re_tk = rtk.refresh_token;
            CacheHelper.Save("wx.refresh_token", rtk.refresh_token);
            return rtk.access_token;
        }
        class tick : Basic.mbase
        {
            public string ticket { get; set; }
            public int expires_in { get; set; }
        }
    }
}
