using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using X.Core.Cache;
using X.Core.Plugin;
using X.Core.Utility;

namespace X.App.Com
{
    public class Wx
    {
        static string appid = "";
        static string appsecret = "";

        public Wx(string id, string sec)
        {
            id = appid;
            appsecret = sec;
        }
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="isnew">
        /// 强制更新
        /// </param>
        /// <returns></returns>
        public string GetToken(bool isnew)
        {
            var tk = CacheHelper.Get<string>("wx.access_token");
            if (string.IsNullOrEmpty(tk) || isnew)
            {
                var json = Tools.GetHttpData("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + appsecret);
                Debug.WriteLine("getToken->" + json);
                var tke = Serialize.FromJson<token>(json);
                tk = tke.access_token;
                CacheHelper.Save("wx.access_token", tk, tke.expires - 500);
            }
            return tk;
        }
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            return GetToken(false);
        }
        /// <summary>
        /// 参数签名
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="backxml"></param>
        /// <param name="mch_key"></param>
        /// <returns></returns>
        public string ToSign(Dictionary<string, string> ps, bool backxml, string mch_key)
        {
            var sign_str = new StringBuilder();
            var xml_data = new StringBuilder();
            xml_data.Append("<xml>");
            foreach (var d in ps.OrderBy(o => o.Key))
            {
                sign_str.Append(d.Key + "=" + d.Value + "&");
                xml_data.Append("<" + d.Key + ">" + d.Value + "</" + d.Key + ">");
            }
            sign_str.Append("key=" + mch_key);
            var sign = Secret.MD5(sign_str.ToString().TrimEnd('&'), 0).ToUpper();
            xml_data.Append("<sign>" + sign + "</sign>");
            xml_data.Append("</xml>");
            return backxml ? xml_data.ToString() : sign;
        }










    }
}
