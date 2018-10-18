using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.pc.draws.reps
{
    public class list : xmg
    {
        public int img { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }
        protected override string PowerCode => "E010509";
        protected override XResp Execute()
        {
            var r = new XList();
            r.page = page;

            var q = from e in db.x_draw_reply
                    select e;

            q = q.OrderByDescending(o => o.ctime);

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.cot.Contains(key) || o.nickname.Contains(key) || o.x_draw.topic.Contains(key) || o.x_draw.no.Contains(key));

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList().Select(e => new
            {
                id = e.draw_reply_id,
                draw = new { e.x_draw.cover, e.x_draw.no, e.x_draw.topic },
                e.cot,
                e.headimg,
                e.nickname,
                e.rep,
                e.reper,
                rtime = e.rtime?.ToString("yyyy-MM-dd HH:mm"),
                e.status,
                e.tel,
                time = e.ctime?.ToString("yyyy-MM-dd HH:mm")
            });
            r.count = q.Count();

            return r;
        }
    }
}
