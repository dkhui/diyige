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
    public class logs : xmg
    {
        public int pid { get; set; }
        public int page { get; set; }
        public int limit { get; set; }

        protected override string PowerCode => "E010215";
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == pid);

            var r = new XList();
            r.page = page;

            var q = from u in p.x_project_log
                    select u;

            var list = q.OrderBy(o => o.ctime).Skip((page - 1) * limit).Take(limit).ToList();

            r.items = list.Select(u => new
            {
                u.x_mgr.name,
                u.cot,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm"),
                u.tname,
                u.remark
            }).ToList();
            r.count = q.Count();

            return r;
        }
    }
}
