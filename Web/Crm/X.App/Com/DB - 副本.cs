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
        public string tname => getDname("|�������|Ч������|�������|�ṹ���|����ˮ���|�������", type ?? -1);
        public string stname => getDname("|������|������|�����|�����|�����޸�|��ȡ��", status ?? -1);
    }
    partial class x_project : ent
    {
        public string stname => getDname("���Ӵ�|Ԥ����|������|���ɵ�|�������|Ч�����|��ϸ���|������|�����", status ?? -1);
    }

    partial class x_ticket : ent
    {
        public string rgname => use_range.Replace("[1]", "��Ʒͼֽ ").Replace("[2]", "���Ʒ��� ").Trim();
        public string ltname => use_limit <= 0 ? "������" : "��" + use_limit + "��ʹ��";// use_range.Replace("[1]", "��Ʒͼֽ").Replace("[2]", "���Ʒ���");
    }

    partial class x_user_ticket : ent
    {
        public string stname => getDname("|��ʹ��|��ʹ��|��ʧЧ", status ?? -1);
    }

    partial class x_order_pay : ent
    {
        public string wname => getDname("|΢��֧��|֧����֧��|΢��ת��|֧����ת��|����ת��|�ֽ�", way ?? -1);
        public string tname => getDname("|ȫ��|�׸���|������|��Ч��|�޸Ŀ�", type ?? -1);
    }

    partial class x_order : ent
    {
        public string stname => getDname(type == 1 ? "������|������|������|��ǩ��|�����" : "������|������|�����|������|���ջ�|�����", status ?? -1);
    }

    partial class x_draw
    {
        public class lay
        {
            static string[] lns = "һ��|����|����|�Ĳ�|���|����".Split('|');
            string getlyname(int id) { if (id == 11) return "������"; else if (id == 10) return "��¥"; else return lns[id - 1]; }
            string _title = "";

            public int id { get; set; }
            /// <summary>
            /// ���
            /// </summary>
            public decimal height { get; set; }
            /// <summary>
            /// ����
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
            /// ����
            /// </summary>
            public string text { get; set; }
            /// <summary>
            /// ƽ��ͼ
            /// </summary>
            public string pic { get; set; }
        }
    }

    public class ent
    {
        /// <summary>
        /// ��ȡʱ��
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetTimeDelay(DateTime st)
        {
            var dt = DateTime.Now;
            var sp = dt - st;
            if (sp.TotalDays > 30) return "����30��";
            else if (sp.TotalDays > 2) return (int)sp.Days + "��ǰ";
            else if (sp.TotalDays == 2) return "ǰ��";
            else if (sp.TotalDays == 1) return "����";
            else if (sp.TotalHours > 3) return "����";
            else if (sp.TotalHours > 0) return (int)sp.Hours + "Сʱǰ";
            else if (sp.TotalMinutes > 0) return (int)sp.Minutes + "����ǰ";
            else return "�ո�";
        }

        public string getDname(string ns, int idx)
        {
            var names = ns.Split('|');
            if (idx >= 0 && idx < names.Length) return names[idx];
            else return "δ֪��" + idx;
        }
    }

    partial class DBDataContext
    {
        public string GetDictName(string code, object value) { return GetDictName(code, value, "��"); }
        /// <summary>
        /// ��ȡ�ֵ�����
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value">
        /// ���ֵ�� , ����
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
        /// ��ȡ�����ֵ�
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
        /// ��ȡ�ֵ��б�
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