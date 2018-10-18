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
                cname = u.name.Split(' ')[0],
                addr = u.bud_addr,
                od = db.x_order.Where(o => o.pid == u.project_id).Select(o => new { o.no, o.amount, o.offam, o.payable, o.paid }).FirstOrDefault(),
                u.mg_man,
                u.ap_man,
                u.stname,
                stop = u.isstop == true ? "是" : "否",
                u.status,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm"),
                task = new
                {
                    t1 = getTask(u, 1),
                    t2 = getTask(u, 2),
                    t3 = getTask(u, 3),
                    t4 = getTask(u, 4),
                    t5 = getTask(u, 5),
                    t6 = getTask(u, 6),
                }
            }).ToList();
            r.count = q.Count();

            return r;
        }

        string getTask(x_project p, int t)
        {
            var tk = p.x_task.FirstOrDefault(o => o.type == t && o.status != 6 && o.status != 7);
            var txt = "";
            var tip = "";
            var st = 0;
            if (tk == null) txt = string.IsNullOrEmpty(p.cot) || p.cot.Contains("[" + t + "]") ? "未分配" : "";
            else
            {
                txt = tk.x_mgr.name;
                tip = tk.stname;
                st = tk.status ?? 0;
            }
            return "<span class='ts" + st + "' title='" + tip + "'>" + txt + "</span>";
        }
    }
}
