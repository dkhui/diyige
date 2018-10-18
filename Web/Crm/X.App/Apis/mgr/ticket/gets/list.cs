using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.ticket.gets
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string tel { get; set; }
        public int tid { get; set; }
        public int st { get; set; }

        protected override XResp Execute()
        {
            var tk = db.x_ticket.FirstOrDefault(o => o.ticket_id == tid);
            if (tk == null) throw new XExcep("T优惠券不存在！");

            var r = new XList();
            r.page = page;

            var q = from u in tk.x_user_ticket select u;
            if (st > 0) q = q.Where(o => o.status == st);
            if (!string.IsNullOrEmpty(tel)) q = q.Where(o => o.tel == tel);

            var list = q.OrderByDescending(o => o.ctime).Skip((page - 1) * limit).Take(limit).ToList();

            r.items = list.Select(u => new
            {
                id = u.user_ticket_id,
                u.amount,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm"),
                name = u.x_user != null ? u.x_user.name + " " + u.x_user.tel : "待用户激活" + u.tel,
                u.stname,
                u.no,
                headimg = u.x_user != null ? u.x_user.headimg : ""
            }).ToList();
            r.count = q.Count();

            return r;
        }

    }
}
