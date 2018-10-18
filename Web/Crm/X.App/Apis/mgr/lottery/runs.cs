using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery
{
    public class runs : xmg
    {
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
                    select lt;

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.title.Contains(key));

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList().Select(u => new
            {
                u.title,
                u.open,
                u.sum,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm")
            }).ToList();
            r.count = q.Count();

            return r;
        }
    }
}
