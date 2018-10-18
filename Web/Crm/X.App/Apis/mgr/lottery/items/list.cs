using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery.items
{
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

            var q = from lt in DB.x_lottery_item
                    where lt.lottery_id == lot_id
                    select lt;

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.name.Contains(key));

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList().Select(o => new
            {
                o.chance,
                id = o.lottery_item_id,
                o.name,
                o.pic,
                o.count,
                o.price,
                o.remark,
                type = getTp(o.type)

            }).ToList();
            r.count = q.Count();

            return r;
        }

        string getTp(int? st)
        {
            if (st == null) return "";
            else if (st == 1) return "积分";
            else if (st == 2) return "实物";
            else return "";
        }
    }
}
