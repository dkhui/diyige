using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.ticket
{
    /// <summary>
    /// 用户管理列表
    /// </summary>
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }

        protected override XResp Execute()
        {
            var r = new XList();
            r.page = page;

            var q = from u in db.x_ticket
                    select u;

            var list = q.OrderByDescending(o => o.ctime).Skip((page - 1) * limit).Take(limit).ToList();

            r.items = list.Select(u => new
            {
                id = u.ticket_id,
                u.count,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm"),
                u.getct,
                u.getlimit,
                getime = u.get_etime?.ToString("yyyy-MM-dd HH:mm"),
                u.ltname,
                u.min_am,
                u.max_am,
                u.status,
                u.type,
                tname = u.type == 1 ? "用户领取" : u.type == 2 ? "后台发放" : "未知",
                stname = u.status == 1 ? "正常" : u.status == 2 ? "停止" : "未知",
                u.remark,
                u.rgname,
                u.topic,
                u.use_limit
            }).ToList();
            r.count = q.Count();

            return r;
        }

    }
}
