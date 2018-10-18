using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.pc.draws
{
    public class list : xmg
    {
        public int cate { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }
        public int sell { get; set; }


        protected override XResp Execute()
        {
            var r = new XList();
            r.page = page;

            var q = from e in db.x_draw
                    select e;

            q = q.OrderByDescending(o => o.mtime);

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.topic.Contains(key) || o.no.Contains(key));
            if (cate > 0) q = q.Where(o => o.cate == cate);
            if (sell == 2) q = q.Where(o => o.sell == true);
            else if (sell == 3) q = q.Where(o => o.sell == false);

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList().Select(e => new
            {
                id = e.draw_id,
                e.cate,
                e.cover,
                pno = e.no + "_" + e.no2 + (e.mirror == true ? "_1" : ""),
                no = e.no,
                sell = e.sell == true ? 1 : 0,
                no2 = db.GetDictName("draw.scheme", e.no2),
                mr = e.mirror == true ? "镜像" : "",
                e.sort,
                e.topic,
                e.price,
                e.attrs,
                deser = e.dutier,
                time = e.mtime?.ToString("yyyy-MM-dd HH:mm")
            });
            r.count = q.Count();

            return r;
        }
    }
}
