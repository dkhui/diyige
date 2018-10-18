using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.proj
{
    /// <summary>
    /// 用户管理列表
    /// </summary>
    public class list : xmg
    {
        public int st { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public long mid { get; set; }
        public string key { get; set; }
        public int tst { get; set; }
        public int tk { get; set; }

        protected override string PowerCode
        {
            get
            {
                if (st == 1) return "E010202";
                else if (st == 2) return "E010203";
                else if (st == 3) return "E010204";
                else if (st == 4) return "E010218";
                else return "E010205";
            }
        }

        protected override XResp Execute()
        {
            var r = new XList();
            r.page = page;

            var q = from u in db.x_project
                    select u;

            if (st == 1) q = q.Where(o => o.status <= 1);
            else if (st == 2) q = q.Where(o => o.status > 1 && o.status < 4);
            else if (st == 3) q = q.Where(o => o.status == 5);
            else if (st == 4) q = q.Where(o => o.status == 4);

            if (tk > 0 && tst > 0) q = q.Where(o => o.x_task.Count(t => t.type == tk && t.status == tst) > 0);

            if (!HasPower("E01020602")) q = q.Where(o => o.ap_id == null || o.ap_id == 0 || o.ap_id == mg.mgr_id || o.x_task.Count(t => t.mgr_id == mg.mgr_id && t.status != 6) > 0);

            if (mid > 0) q = q.Where(o => o.mg_id == mid);
            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.rec_addr.Contains(key) || o.no == key || o.name.Contains(key));

            var list = q.OrderBy(o => o.status).Skip((page - 1) * limit).Take(limit).ToList();

            r.items = list.Select(u => new
            {
                id = u.project_id,
                u.no,
                cname = u.name,
                u.bud_addr,
                u.rec_addr,
                od = db.x_order.Where(o => o.pid == u.project_id).Select(o => new { o.no, o.amount, o.offam, o.payable, o.paid }).FirstOrDefault(),
                u.mg_man,
                u.ap_man,
                u.stname,
                stop = u.isstop == true ? "是" : "否",
                u.status,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm"),
                task = getTask(u)
            }).ToList();
            r.count = q.Count();

            return r;
        }

        string getTask(x_project p)
        {
            var ls = db.GetDictList("task.type", "0", p.cot).OrderBy(o => o.value);
            var sb = new StringBuilder();
            foreach (var l in ls)
            {
                var ts = p.x_task.Where(o => (o.type + "") == l.value && o.status != 6 && o.status != 7);
                if (ts.Count() == 0) sb.Append("<span class='ts0'>" + l.name + "：未分配</span>");
                foreach (var t in ts)
                    sb.Append("<span class='ts" + t.status + "' title='" + t.x_mgr.name + "'>" + l.name + "：" + t.stname + "</span>");
            }
            return sb.ToString();
        }
    }
}
