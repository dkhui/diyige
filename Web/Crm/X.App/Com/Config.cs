using System.Collections.Generic;
using System.Web;
using X.Core.Utility;

namespace X.App.Com
{
    /// <summary>
    /// 常规配置
    /// </summary>
    public class Config
    {
        #region 基本配置
        /// <summary>
        /// 域名
        /// </summary>
        public string domain { get; set; }
        /// <summary>
        /// 网站名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 缓存设置
        /// 1、memcached
        /// 2、WebCached
        /// </summary>
        public int cache { get; set; }
        #endregion

        #region 微信配置
        public wxcfg wx { get; set; }
        public class wxcfg
        {
            public string appid { get; set; }
            public string secret { get; set; }
        }
        #endregion

        #region 支付配置
        public paycfg pay { get; set; }
        public class paycfg
        {
            public string ways { get; set; }
            /// <summary>
            /// 1、微信
            /// 2、支付宝
            /// </summary>
            public alicfg ali { get; set; }
            public wxcfg wx { get; set; }
            public class alicfg
            {
                public string appid { get; set; }
                public string mch_key { get; set; }
                public string pub_key { get; set; }
                public string ali_key { get; set; }
                public string gateway { get; set; }
            }
            public class wxcfg
            {
                public string mchid { get; set; }
                public string certpath { get; set; }
                public string paykey { get; set; }
            }
            public paycfg()
            {
                ways = "[1][2]";
                ali = new alicfg();
                wx = new wxcfg();
            }
        }
        #endregion

        #region 短信配置
        public smscfg sms { get; set; }
        public class smscfg
        {
            /// <summary>
            /// 1、阿里云
            /// 2、创蓝
            /// </summary>
            public int way { get; set; }
            public alicfg ali { get; set; }
            public clancfg clan { get; set; }
            public List<tpl> tpls { get; set; }
            public class alicfg
            {
                public string endpoint { get; set; }
                public string appid { get; set; }
                public string secret { get; set; }
                public string topic { get; set; }
                public string sign { get; set; }
            }
            public class clancfg
            {
                public string account { get; set; }
                public string password { get; set; }
                public string gateway { get; set; }
                public string sign { get; set; }
            }
            public class tpl
            {
                /// <summary>
                /// 模板代号
                /// </summary>
                public string code { get; set; }
                /// <summary>
                /// 模板标题
                /// </summary>
                public string title { get; set; }
                /// <summary>
                /// 模板内容
                /// </summary>
                public string content { get; set; }
                /// <summary>
                /// 阿里模板编号
                /// </summary>
                public string alicode { get; set; }
                public string parms { get; set; }
            }
            public smscfg()
            {
                ali = new alicfg();
                clan = new clancfg();
                tpls = new List<tpl>();
            }
        }
        #endregion

        #region 水印配置
        #endregion

        private static string file = HttpContext.Current.Server.MapPath("/dat/cfg.x");//来自服务器的文件
        private static Config cfg = null;

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static Config LoadConfig()
        {
            if (cfg == null)
            {
                var json = Tools.ReadFile(file);
                if (string.IsNullOrEmpty(json)) return new Config();
                cfg = Serialize.FromJson<Config>(json);
            }
            return cfg;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="cfg"></param>
        public static void SaveConfig(Config cfg)
        {
            Tools.SaveFile(HttpContext.Current.Server.MapPath("/dat/cfg.x"), Serialize.ToJson(cfg));
        }
    }
}
