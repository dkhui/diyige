using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Web.Com;

namespace X.App.Apis.pc.order
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public int st { get; set; }
        public int tp { get; set; }
        public string key { get; set; }

        protected override XResp Execute()
        {
            var r = new XList();
            var q = from o in db.x_order
                    select o;

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.no == key || o.imgno.Contains(key) || o.user_remark.Contains(key) || o.rec_man.Contains(key) || o.rec_tel.Contains(key));

            if (st == 1) q = q.Where(o => (o.type == 1 && o.status == 2) || (o.type == 2 && o.status == 3));
            else if (st == 2) q = q.Where(o => (o.paid < o.payable || (o.type == 2 && o.status <= 3)) && o.payable > 0);
            else if (st == 3) q = q.Where(o => (o.type == 1 && o.status == 3) || (o.type == 2 && o.status == 4));
            if (tp > 0) q = q.Where(o => o.type == tp);

            r.count = q.Count();
            r.items = q.OrderByDescending(o => o.ctime).Skip((page - 1) * limit).Take(limit).ToList().Select(o => new
            {
                id = o.order_id,
                uid = o.user_id,
                un = o.x_user.name,
                up = o.x_user.headimg,
                o.no,
                o.type,
                o.amount,
                o.offam,
                o.rec_man,
                o.rec_tel,
                o.rec_addr,
                o.payable,
                o.paid,
                gs = new { cover = o.cover, no = o.imgno, topic = o.topic },
                o.status,
                stname = o.stname,
                o.track_com,
                o.track_no,
                o.send_man,
                send_time = o.send_time?.ToString("yyyy-MM-dd HH:mm:ss"),
                ctime = o.ctime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                remark = o.user_remark
            });

            r.page = page;

            return r;
        }
    }
}
