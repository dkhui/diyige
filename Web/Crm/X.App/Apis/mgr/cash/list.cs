using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.mgr.cash
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }
        public int st { get; set; }

        protected override int powercode
        {
            get
            {
                return 1;
            }
        }

        protected override Web.Com.XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from d in DB.x_cash_log
                    select d;

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.remark.Contains(key));
            if (st == 2) q = q.Where(o => o.status == 1);
            else if (st == 3) q = q.Where(o => o.status == 2);

            q = q.OrderBy(o => o.status).ThenByDescending(o => o.ctime).Skip((page - 1) * limit).Take(limit);

            r.items = q.ToList().Select(d => new
            {
                id = d.cash_log_id,
                amount = d.amount,
                uid = d.x_user.user_id,
                name = d.x_user.name,
                un = d.x_user.nickname,
                up = d.x_user.headimg,
                ctime = d.ctime?.ToString("yyyy-MM-dd HH:mm"),
                remark = d.remark,
                status = d.status,
                statusname = d.status == 3 ? "已拒绝" : (d.status == 2 ? "已同意" : "待审核")
            });
            r.count = q.Count();
            return r;
        }

    }
}
