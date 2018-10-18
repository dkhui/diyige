using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.proj
{
    /// <summary>
    /// 用户管理列表
    /// </summary>
    public class task : xmg
    {
        public int st { get; set; }
        public int pid { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        protected override string PowerCode => "E010214";

        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == pid);

            var r = new XList();
            r.page = page;

            var q = from u in p.x_task
                    select u;

            if (st > 1) q = q.Where(o => o.status == st - 1);

            var list = q.OrderByDescending(o => o.mgr_id).Skip((page - 1) * limit).Take(limit).ToList();

            r.items = list.Select(u => new
            {
                id = u.task_id,
                pid = u.project_id,
                u.content,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm"),
                etime = u.etime?.ToString("yyyy-MM-dd HH:mm"),
                ptime = u.ptime?.ToString("yyyy-MM-dd HH:mm"),
                u.name,
                u.opusr,
                u.remark,
                u.status,
                toname = u.x_mgr.name,
                totel = u.x_mgr.tel,
                u.stname,
                user = u.x_project.name,
                u.type,
                tname = db.GetDictName("task.type", u.type)
            }).ToList();
            r.count = q.Count();

            return r;
        }
    }
}
