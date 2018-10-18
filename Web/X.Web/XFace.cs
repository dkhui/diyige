using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Web;
using X.Web.Cache;

namespace X.Web
{
    public abstract class XFace
    {
        /// <summary>
        /// 1、PC
        /// 2、WX
        /// 3、MB
        /// </summary>
        protected int platform = 1;
        /// <summary>
        /// 会话缓存
        /// </summary>
        protected SessionCache Session = null;
        /// <summary>
        /// 上下文缓存
        /// </summary>
        protected ContextCache XCache = null;
        /// <summary>
        /// Http上下文
        /// </summary>
        protected HttpContext Context = null;
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(HttpContext c)
        {
            Session = new SessionCache();
            XCache = new ContextCache();
            Context = c;

            if (Context.Request.UserAgent != null)
            {
                var ua = Context.Request.UserAgent.ToLower();
                if (ua.Contains("micromessenger")) platform = 2;
                else if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone") || ua.Contains("ipad") || ua.Contains("phone")) platform = 3;
                else platform = 1;
            }
            else if (Context.Request.Params["platform"] != null)
                int.TryParse(Context.Request.Params["platform"], out platform);

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="parms"></param>
        protected void SetParms(NameValueCollection parms)
        {
            var postes = parms;
            var type = GetType();

            foreach (var p in type.GetProperties())
            {
                var obj = p.GetValue(this, null) + "";
                if (!string.IsNullOrEmpty(obj) && obj != "0" && !obj.Equals(DateTime.MinValue.ToString())) continue;
                //if (obj != null || (obj + "" != "0")) continue;
                var pa = new ParmsAttr();
                var attr = p.GetCustomAttributes(false);
                if (attr.Length > 0) pa = attr[0] as ParmsAttr;

                object v = null;
                var k = p.PropertyType.Name.ToLower();
                var n = p.Name.ToLower();
                switch (k)
                {
                    case "string":
                        var sv = Context.Server.HtmlEncode(postes[n]);
                        if (string.IsNullOrEmpty(sv) && pa.def != null) sv = pa.def + "";
                        Checker.check(pa, sv);
                        v = sv;
                        break;
                    case "int":
                    case "int32":
                        var iv = 0;
                        int.TryParse(postes[n], out iv);
                        if (iv == 0 && pa.def != null && pa.def.GetType() == typeof(int)) iv = Convert.ToInt16(pa.def);
                        Checker.check(pa, iv);
                        v = iv;
                        break;
                    case "int64":
                        long lv = 0;
                        long.TryParse(postes[n], out lv);
                        if (lv == 0 && pa.def != null && pa.def.GetType() == typeof(long)) lv = Convert.ToInt64(pa.def);
                        Checker.check(pa, lv);
                        v = lv;
                        break;
                    case "decimal":
                        var fv = (decimal)0.0;
                        decimal.TryParse(postes[n], out fv);
                        if (fv == 0 && pa.def != null && pa.def.GetType() == typeof(decimal)) fv = Convert.ToDecimal(pa.def);
                        Checker.check(pa, fv);
                        v = fv;
                        break;
                    case "datetime":
                        DateTime dv = DateTime.MinValue;
                        DateTime.TryParse(postes[n], out dv);
                        if (dv == DateTime.MinValue && pa.def != null && pa.def.GetType() == typeof(DateTime)) dv = Convert.ToDateTime(pa.def);
                        Checker.check(pa, dv);
                        v = dv;
                        break;
                    default:
                        continue;
                }
                if (v != null) p.SetValue(this, v, null);
            }
        }

        /// <summary>
        /// 参数检查器
        /// </summary>
        class Checker
        {
            public static void check(ParmsAttr pa, string v)
            {
                if (pa.req && string.IsNullOrEmpty(v)) throw new XExcep("0x0003", pa.name);

                string min = pa.min + "";
                string max = pa.max + "";
                if (!string.IsNullOrEmpty(min) && v.CompareTo(min) < 0) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (!string.IsNullOrEmpty(max) && v.CompareTo(max) > 0) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));

                if (string.IsNullOrEmpty(pa.len)) return;
                var ls = pa.len.Split(',');
                if (ls.Length == 1 && v.Length != int.Parse(ls[0])) throw new XExcep("0x0004", String.Format("{0}应为{1}个字符", pa.name, ls[0]));
                else if (ls.Length > 1)
                {
                    if (!string.IsNullOrEmpty(ls[0]) && v.Length < int.Parse(ls[0])) throw new XExcep("0x0004", String.Format("{0}至少{1}个字符", pa.name, ls[0]));
                    if (!string.IsNullOrEmpty(ls[1]) && v.Length > int.Parse(ls[1])) throw new XExcep("0x0004", String.Format("{0}最多{1}个字符", pa.name, ls[1]));
                }
            }
            public static void check(ParmsAttr pa, int v)
            {
                if (pa.req && v == 0) throw new XExcep("0x0003", pa.name);

                int? min = null, max = null;
                min = Convert.ToInt32(pa.min);
                max = Convert.ToInt32(pa.max);

                if (pa.min != null && v < min) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (pa.max != null && v > max) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));
            }
            public static void check(ParmsAttr pa, decimal v)
            {
                if (pa.req && v == 0) throw new XExcep("0x0003", pa.name);

                if (pa.min == null && pa.max == null) return;
                decimal min = Convert.ToDecimal(pa.min);
                decimal max = Convert.ToDecimal(pa.max);
                if (v < min && pa.min != null) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (v > max && pa.max != null) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));
            }
            public static void check(ParmsAttr pa, long v)
            {
                if (pa.def != null && v == 0) v = (long)pa.def;
                if (pa.req && v == 0) throw new XExcep("0x0003", pa.name);

                if (pa.min == null && pa.max == null) return;
                long min = Convert.ToInt64(pa.min);
                long max = Convert.ToInt64(pa.max);
                if (v < min && pa.min != null) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (v > max && pa.max != null) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));
            }
            public static void check(ParmsAttr pa, DateTime v)
            {
                if (pa.def != null && v == DateTime.MinValue) v = (DateTime)pa.def;
                if (pa.req && v == null) throw new XExcep("0x0003", pa.name);

                if (pa.min == null && pa.max == null) return;
                DateTime min = Convert.ToDateTime(pa.min);
                DateTime max = Convert.ToDateTime(pa.max);
                if (v < min && pa.min != null) throw new XExcep("0x0004", String.Format("{0}的值要大于{1}", pa.name, min));
                if (v > max && pa.max != null) throw new XExcep("0x0004", String.Format("{0}的值要小于{1}", pa.name, max));
            }
        }

        /// <summary>
        /// 获取格式化日期
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string GetDateString(DateTime? dt, string format)
        {
            if (dt == null) return "";
            else return dt.Value.ToString(format);
        }

        /// <summary>
        /// 获取响应
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetResponse();
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetReqParms(string key)
        {
            try
            {
                return Context.Request.Params.Get(key) ?? "";
            }
            catch { return ""; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        protected class ParmsAttr : Attribute
        {
            public string name { get; set; }
            /// <summary>
            /// 字符串不许空
            /// 数字不为0
            /// </summary>
            public bool req { get; set; }
            /// <summary>
            /// 长度min,max
            /// </summary>
            public string len { get; set; }
            /// <summary>
            /// 最小值
            /// </summary>
            public object min { get; set; }
            /// <summary>
            /// 最大值
            /// </summary>
            public object max { get; set; }
            /// <summary>
            /// 默认值
            /// </summary>
            public object def { get; set; }
        }
    }
}
