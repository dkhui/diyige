using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.pc.commission
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override XResp Execute()
        {
            var r = new XList();
            r.page = page;

            var q = from e in db.x_commission
                    select e;

            if (!string.IsNullOrEmpty(key))
                q = q.Where(o => o.name.Contains(key) || o.remark.Contains(key));

            r.items = q.Skip((page - 1) * limit)
                .Take(limit)
                .ToList()
                .Select(ad => new
                {
                    id = ad.commission_id,
                    ad.name,
                    ad.remark,
                    frm = db.GetDictName("draw.frame", ad.frame, "<br>"),
                    cot = db.GetDictName("task.type", ad.cot, "<br>"),
                    dt = ad.dt?.ToString("yyyy年MM月dd日"),
                    fa = ad.fa?.ToString("0.##") + (ad.fa > 50 ? "/单" : "%"),
                    jg = ad.jg?.ToString("0.##") + (ad.jg > 50 ? "/单" : "%"),
                    jz = ad.jz?.ToString("0.##") + (ad.jz > 50 ? "/单" : "%"),
                    sl = ad.sl?.ToString("0.##") + (ad.sl > 50 ? "/单" : "%"),
                    dq = ad.dq?.ToString("0.##") + (ad.dq > 50 ? "/单" : "%"),
                    xg = ad.xg?.ToString("0.##") + (ad.xg > 50 ? "/单" : "%")
                });
            r.count = q.Count();
            return r;
        }

    }
}
