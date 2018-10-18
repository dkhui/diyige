using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.task
{
    /// <summary>
    /// 用户管理列表
    /// </summary>
    public class list : xmg
    {
        public int st { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public int tp { get; set; }
        public string key { get; set; }
        public int ism { get; set; }

        protected override string PowerCode
        {
            get
            {
                if (st == 1) return "E010301";
                else if (st == 2) return "E010302";
                else if (st == 3) return "E010303";
                else return "E010304";
            }
        }

        protected override XResp Execute()
        {
            var r = new XList();
            r.page = page;

            var q = from u in db.x_task
                    select u;

            if (st == 2) q = q.Where(o => o.status == 2 || o.status == 5);
            else if (st == 1) q = q.Where(o => o.status == 1);
            else if (st == 3) q = q.Where(o => o.status == 4);
            else if (st == 4) q = q.Where(o => o.status == 3);

            if (tp > 0) q = q.Where(o => o.type == tp);
            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.x_project.no == key || o.x_mgr.name.Contains(key) || o.x_project.name.Contains(key));

            if (!HasPower("E01030502") || ism == 1) if (st == 4) q = q.Where(o => o.x_project.mg_id == mg.mgr_id); else q = q.Where(o => o.mgr_id == mg.mgr_id);

            var list = q.OrderByDescending(o => o.x_project.user_id).Skip((page - 1) * limit).Take(limit).ToList();

            r.items = list.Select(u => new
            {
                id = u.task_id,
                pid = u.project_id,
                cname = u.x_project.name.Split(' ')[0],
                u.x_project.no,
                tname = db.GetDictName("task.type", u.type),
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm"),
                etime = u.etime?.ToString("yyyy-MM-dd HH:mm"),
                ptime = u.ptime?.ToString("yyyy-MM-dd HH:mm"),
                u.x_project.mg_man,
                u.name,
                u.opusr,
                u.remark,
                u.status,
                toname = u.x_mgr.name,
                totel = u.x_mgr.tel,
                u.stname,
                u.type
            }).ToList();
            r.count = q.Count();

            return r;
        }
    }
}
