using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery.records
{
    /// <summary>
    /// 用户管理列表
    /// </summary>
    public class list : xmg
    {
        public int lot_id { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override int powercode
        {
            get
            {
                return 1;
            }
        }

        protected override XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from lt in DB.x_lottery_run
                    where lt.lottery_id == lot_id
                    select lt;

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.title.Contains(key));

            r.items = q.OrderByDescending(o => o.ctime).Skip((page - 1) * limit).Take(limit).ToList().Select(u => new
            {
                id = u.lottery_run_id,
                u.title,
                sum = u.sum?.ToString("F2"),
                u.open,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm")
            }).ToList();
            r.count = q.Count();

            return r;
        }

    }
}
