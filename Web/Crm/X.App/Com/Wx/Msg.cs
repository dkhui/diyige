using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using X.Core.Plugin;
using X.Core.Utility;

namespace X.App.Com.Wx
{
    /// <summary>
    /// 消息
    /// </summary>
    public class Msg
    {
        public MsgObj Get(string tk_xml, string sign, string nonce, string timestamp)
        {
            try
            {
                var xml = tk_xml;
                if (xml.IndexOf("<Encrypt>") > 0) xml = Crypt.DecryptMsg(Basic.appid, sign, timestamp, nonce, xml);
                var obj = new MsgObj(xml);
                return obj;
            }
            catch (WxExcep wex)
            {
                Loger.Error(wex);
                return null;
            }
        }

        public bool SendTpl(string tk, string opid, string tplid, string url, Dictionary<string, Tplmsg> ps)
        {
            var api = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + tk;
            var data = new StringBuilder();
            data.Append("{\"touser\":\"" + opid + "\",\"template_id\":\"" + tplid + "\",\"url\":\"" + url + "\",\"data\":{ ");

            if (ps.ContainsKey("first")) data.Append("\"first\":{\"value\":\"" + ps["first"].value + "\",\"color\":\"" + ps["first"].color + "\"},");

            foreach (var p in ps.Where(o => o.Key != "first" && o.Key != "remark"))
                data.Append("\"" + p.Key + "\":{\"value\":\"" + p.Value.value + "\",\"color\":\"" + p.Value.color + "\"},");

            if (ps.ContainsKey("remark")) data.Append("\"remark\":{\"value\":\"" + ps["remark"].value + "\",\"color\":\"" + ps["remark"].color + "\"}");
            else data = data.Remove(data.Length - 2, 1);

            data.Append("}}");
            var back = Serialize.FromJson<Basic.mbase>(Tools.PostHttpData(api, data.ToString()));
            Loger.Info("wx.msg.sendtpl->" + Serialize.ToJson(back));
            return back.errmsg == "ok";
        }

        public enum Tplkey
        {
            Key,
            First,
            Remark
        }
        public class Tplmsg
        {
            public string value { get; set; }
            public string color { get; set; }
            public Tplmsg(string value)
            {
                this.value = value;
                this.color = "#000000";
            }
            public Tplmsg(string value, string color)
            {
                this.value = value;
                this.color = color;
            }
        }
        public class MsgObj
        {
            public string FromUserName { get { return GetString("FromUserName"); } }
            public string ToUserName { get { return GetString("ToUserName"); } }
            public int CreateTime { get { return GetInt("CreateTime"); } }
            public string MsgType { get { return GetString("MsgType"); } }

            private Dictionary<string, string> dict = new Dictionary<string, string>();

            public string GetString(string name)
            {
                if (dict.ContainsKey(name)) return dict[name];
                return "";
            }

            public int GetInt(string name)
            {
                if (dict.ContainsKey(name)) return dict[name] == null ? 0 : int.Parse(dict[name]);
                return 0;
            }

            /// <summary>
            /// Initializes a new instance of the MsgObj class.
            /// </summary>
            public MsgObj(string xml)
            {
                if (string.IsNullOrEmpty(xml)) return;
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                var root = doc.FirstChild;
                foreach (XmlNode n in root.ChildNodes)
                {
                    var v = "";
                    if (n.NodeType == XmlNodeType.CDATA) v = n.FirstChild.InnerText;
                    else v = n.InnerText;
                    dict.Add(n.Name, v);
                }
            }

            public string ToXml(string appid)
            {
                var sb_str = new StringBuilder();
                sb_str.Append("<xml>");
                foreach (var k in dict.Keys)
                {
                    if ("CreateTime|Latitude|Longitude|Precision".IndexOf(k) < 0) sb_str.Append("<" + k + "><![CDATA[" + dict[k] + "]]></" + k + ">");
                    else sb_str.Append("<" + k + ">" + dict[k] + "</" + k + ">");
                }
                sb_str.Append("</xml>");
                return Crypt.EncryptMsg(appid, sb_str.ToString(), Tools.GetGreenTime(""), Tools.GetRandRom(9, 3));
            }

            public void AddValue(string name, string value)
            {
                dict.Add(name, value);
            }

        }
    }
}
