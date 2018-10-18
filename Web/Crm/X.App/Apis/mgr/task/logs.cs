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
    public class logs : xmg
    {
        public int tid { get; set; }
        public int page { get; set; }
        public int limit { get; set; }

        protected override string PowerCode => "E010312";
        protected override XResp Execute()
        {
            var p = db.x_task.FirstOrDefault(o => o.task_id == tid);

            var r = new XList();
            r.page = page;

            var q = from u in p.x_task_log
                    select u;

            var list = q.OrderBy(o => o.ctime).Skip((page - 1) * limit).Take(limit).ToList();

            r.items = list.Select(u => new
            {
                mname = u.x_mgr.name,
                u.remark,
                u.sname,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm")
            }).ToList();
            r.count = q.Count();

            return r;
        }
    }
}
