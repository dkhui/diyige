using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using X.Core.Plugin;
using X.Core.Utility;
using X.Web.Com;

namespace X.Web.Views
{
    public abstract class View : XFace
    {
        /// <summary>
        /// 静态化时长 分钟
        /// 0 为不静态化
        /// -1 为永久静态化
        /// </summary>
        protected virtual int html_time
        {
            get { return 0; }
        }
        /// <summary>
        /// 获取页面参数
        /// </summary>
        protected void GetPageParms()
        {
            if (string.IsNullOrEmpty(GetParmNames)) return;

            var ns = GetParmNames.Split('-');
            var vs = GetReqParms("p").Split('-');

            var ps = new NameValueCollection();
            for (var i = 0; i < ns.Length; i++)
            {
                if (i >= vs.Length) { ps.Add(ns[i], ""); continue; }
                dict.Add(ns[i], Context.Server.UrlDecode(vs[i]));
                ps.Add(ns[i], vs[i]);
            }

            SetParms(ps);
        }
        /// <summary>
        /// 模板引擎数据字典
        /// </summary>
        public TplDict dict = new TplDict();

        protected virtual void InitDict()
        {
            dict.Add("T", this);
            dict.Add("platform", platform);
        }

        protected virtual string GetParmNames
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 初始化视图
        /// </summary>
        protected virtual void InitView()
        {
            GetPageParms();
        }

        public virtual string GetTplFile()
        {
            return GetType().FullName.Replace("X.App.Views.", string.Empty).Replace(".", "/");
        }

        /// <summary>
        /// 执行结果
        /// </summary>
        /// <returns></returns>
        public override byte[] GetResponse()
        {
            var qs = Context.Request.QueryString;
            var v = qs["v"];

            var ht = "/html/" + v;
            ht += ((!string.IsNullOrEmpty(qs["p"]) ? "-" + qs["p"] : "") + ".html").ToLower();

            //var file = Context.Server.MapPath(Secret.MD5(ht)); //Context.Request.RawUrl;

            //if (html_time > 0 || html_time == -1)
            //{
            //    var fi = new FileInfo(file);
            //    if (fi.Exists && (DateTime.Now - fi.LastWriteTime).TotalMinutes < html_time) return File.ReadAllBytes(file);
            //}
            var dt = DateTime.Now;
            InitView();
            //Loger.Info("view->initview->" + (DateTime.Now - dt).TotalMilliseconds);
            InitDict();
            //Loger.Info("view->initdict->" + (DateTime.Now - dt).TotalMilliseconds);

            var html = "";
            try
            {
                html = Tpl.Instance.Merge(GetTplFile() + ".html", dict);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is XExcep)
                {
                    throw (XExcep)ex.InnerException;
                }
                throw ex;
            }
            //Loger.Info("view->tpl->" + (DateTime.Now - dt).TotalMilliseconds);

            dict.Clear();

            XForm.GetListData += XForm_GetListData;
            html = XForm.Parse(html);
            //Loger.Info("view->form->" + (DateTime.Now - dt).TotalMilliseconds);

            #region 压缩页面
            //html = Regex.Replace(html, "(/\\*([^*]|[\r\n]|(\\*+([^*/]|[\r\n])))*\\*+/)|([^:]//.*)", "");
            //html = Regex.Replace(html, "\\s{2,}", " ");//(>)?\\s+< //去掉空格
            #endregion

            var data = Encoding.UTF8.GetBytes(html);

            //if (html_time > 0 || html_time == -1)
            //{
            //    try
            //    {
            //        Directory.CreateDirectory(file.Substring(0, file.LastIndexOf('\\')));
            //        File.WriteAllBytes(file, data);
            //    }
            //    catch { }
            //}

            return data;

        }

        private Dictionary<string, string> XForm_GetListData(string cd, string up)
        {
            return GetDataList(cd, up);
        }

        protected abstract Dictionary<string, string> GetDataList(string cd, string up);
    }
}
