using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery
{
    public class logs : xmg
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

            var q = from lt in DB.x_lottery_log
                    select lt;

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList().Select(u => new
            {
                no = u.x_order.no,
                oamount = u.x_order.yf_amount,
                u.amount,
                u.rate,
                ctime = u.ctime?.ToString("yyyy-MM-dd HH:mm")
            }).ToList();
            r.count = q.Count();

            return r;
        }
    }
}
