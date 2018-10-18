using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using X.Core.Cache;

namespace X.App.Com
{
    partial class x_task : ent
    {
        public string tname => getDname("|方案设计|效果制作|建筑设计|结构设计|给排水设计|电气设计", type ?? -1);
        public string stname => getDname("|待接受|进行中|待审核|已完成|发回修改|已取消", status ?? -1);
    }
    partial class x_project : ent
    {
        public string stname => getDname("待接待|预立项|已立项|已派单|方案设计|效果设计|详细设计|设计完成|已完成", status ?? -1);
    }

    partial class x_ticket : ent
    {
        public string rgname => use_range.Replace("[1]", "成品图纸 ").Replace("[2]", "定制服务 ").Trim();
        public string ltname => use_limit <= 0 ? "无限制" : "满" + use_limit + "可使用";// use_range.Replace("[1]", "成品图纸").Replace("[2]", "定制服务");
    }

    partial class x_user_ticket : ent
    {
        public string stname => getDname("|待使用|已使用|已失效", status ?? -1);
    }

    partial class x_order_pay : ent
    {
        public string wname => getDname("|微信支付|支付宝支付|微信转帐|支付宝转帐|银行转帐|现金", way ?? -1);
        public string tname => getDname("|全款|首付款|定方案|定效果|修改款", type ?? -1);
    }

    partial class x_order : ent
    {
        public string stname => getDname(type == 1 ? "待生成|待付款|待发货|待签收|已完成" : "立项中|已立项|设计中|待发货|待收货|已完成", status ?? -1);
    }

    partial class x_draw
    {
        public class lay
        {
            static string[] lns = "一层|二层|三层|四层|五层|六层".Split('|');
            string getlyname(int id) { if (id == 11) return "地下室"; else if (id == 10) return "阁楼"; else return lns[id - 1]; }
            string _title = "";

            public int id { get; set; }
            /// <summary>
            /// 层高
            /// </summary>
            public decimal height { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string title
            {
                get
                {
                    if (!string.IsNullOrEmpty(_title)) return _title;
                    else return getlyname(id);
                }
                set { _title = value; }
            }
            /// <summary>
            /// 内容
            /// </summary>
            public string text { get; set; }
            /// <summary>
            /// 平面图
            /// </summary>
            public string pic { get; set; }
        }
    }

    public class ent
    {
        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetTimeDelay(DateTime st)
        {
            var dt = DateTime.Now;
            var sp = dt - st;
            if (sp.TotalDays > 30) return "大于30天";
            else if (sp.TotalDays > 2) return (int)sp.Days + "天前";
            else if (sp.TotalDays == 2) return "前天";
            else if (sp.TotalDays == 1) return "昨天";
            else if (sp.TotalHours > 3) return "今天";
            else if (sp.TotalHours > 0) return (int)sp.Hours + "小时前";
            else if (sp.TotalMinutes > 0) return (int)sp.Minutes + "分钟前";
            else return "刚刚";
        }

        public string getDname(string ns, int idx)
        {
            var names = ns.Split('|');
            if (idx >= 0 && idx < names.Length) return names[idx];
            else return "未知：" + idx;
        }
    }

    partial class DBDataContext
    {
        public string GetDictName(string code, object value) { return GetDictName(code, value, "、"); }
        /// <summary>
        /// 获取字典文字
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value">
        /// 多个值用 , 隔开
        /// </param>
        /// <returns></returns>
        public string GetDictName(string code, object value, string split)
        {
            if (value == null || string.IsNullOrEmpty(code)) return string.Empty;

            var val = (value + "").Trim(); //.Split(',');
            var list = GetDictList(code, "", val);
            if (list == null || list.Count == 0) return string.Empty;
            var ns = string.Empty;
            foreach (var d in list)
            {
                if (!string.IsNullOrEmpty(ns)) ns += split;
                ns += d.name;
            }
            return ns;
        }
        /// <summary>
        /// 获取单个字典
        /// </summary>
        /// <param name="code"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public x_dict GetDict(string code, string val)
        {
            var list = GetDictList(code, "", val);
            return list.FirstOrDefault(o => o.value == val);
        }
        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="upval"></param>
        /// <returns></returns>
        public List<x_dict> GetDictList(string code, string upval) { return GetDictList(code, upval, ""); }
        public List<x_dict> GetDictList(string code, string upval, string vals)
        {
            var dt = DateTime.Now;
            var key = "dict." + code;
            List<x_dict> list = null;

            var ct = x_dict.Count(o => o.code == code);

            var q = from c in x_dict
                    where c.code == code && c.value != null
                    select c;

            if (ct > 5000)
                key += "." + upval;

            if (string.IsNullOrEmpty(vals))
                list = CacheHelper.Get<List<x_dict>>(key);

            if (list == null || list.Count == 0)
            {
                if (string.IsNullOrEmpty(vals))
                {
                    if (string.IsNullOrEmpty(upval) || upval == "0") q = q.Where(o => o.upval.Equals("0"));// return list.FindAll(o => { return o.upval == "0"; });
                    else if (upval != "00")
                    {
                        var u = q.FirstOrDefault(o => o.value == upval.Split('-').Last());
                        q = q.Where(o => o.upval == (u.upval == "0" ? u.value : u.upval + "-" + u.value));
                    }
                }
                else
                {
                    q = q.Where(o => vals.IndexOf("[" + o.value + "" + "]") >= 0 || ("," + vals + ",").IndexOf("," + o.value + ",") >= 0);
                }
                list = q.ToList();
                if (string.IsNullOrEmpty(vals))
                    CacheHelper.Save(key, list);
            }

            //Debug.WriteLine("dict." + code + "." + upval + "." + vals + "->" + (DateTime.Now - dt).TotalMilliseconds);
            return list;

            //else
            //{
            //    list = CacheHelper.Get<List<x_dict>>(key);
            //    if (list == null || list.Count == 0)
            //    {
            //        var q = from c in x_dict
            //                where c.code == code && c.value != null
            //                select c;

            //        if (!string.IsNullOrEmpty(vals)) q = q.Where(o => vals.Contains("[" + o.value + "]") || vals.Split(',').Contains(o.value));

            //        list = q.OrderByDescending(o => o.sort).ThenBy(o => o.dict_id).ToList();
            //        if (list == null || list.Count == 0) return null;
            //        if (string.IsNullOrEmpty(vals)) CacheHelper.Save(key, list);
            //    }
            //}


            //if (!string.IsNullOrEmpty(vals)) q = q.Where(o => vals.Contains("[" + o.value + "]") || vals.Split(',').Contains(o.value));
            //else
            //{

            //}

            //list = q.OrderByDescending(o => o.sort).ThenBy(o => o.dict_id).ToList();
            //if (list == null || list.Count == 0) return null;
            //if (string.IsNullOrEmpty(vals)) CacheHelper.Save(key, list);
            //}

            //if (vals == null || vals.Length == 0)

            //    var list = CacheHelper.Get<List<x_dict>>(key);
            //if (list == null || list.Count == 0)
            //{
            //    var q = from c in x_dict
            //            where c.code == code && c.value != null
            //            select c;
            //    list = q.OrderByDescending(o => o.sort).ThenBy(o => o.dict_id).ToList();
            //    if (list == null || list.Count == 0) return null;
            //    CacheHelper.Save(key, list);
            //}
            //            if (upval == "00") return list;
            //            if (string.IsNullOrEmpty(upval) || upval == "0") return list.FindAll(o => { return o.upval == "0"; });
            //            else
            //            {
            //                var u = list.FirstOrDefault(o => o.value == upval.Split('-').Last());
            //                return list.FindAll(o => { return o.upval == (u.upval == "0" ? u.value : u.upval + "-" + u.value);
            //});
            //            }
        }

        public new void SubmitChanges()
        {
            try
            {
                SubmitChanges(ConflictMode.ContinueOnConflict);
                SubmitChanges(ConflictMode.FailOnFirstConflict);
            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict occ in ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }
        }
    }
}