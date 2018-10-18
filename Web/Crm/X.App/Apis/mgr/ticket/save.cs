using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.ticket
{
    public class save : xmg
    {
        public int id { get; set; }
        public int type { get; set; }
        public string topic { get; set; }
        public decimal xam { get; set; }
        public decimal dam { get; set; }
        public string urg { get; set; }
        public decimal umt { get; set; }
        public DateTime gbt { get; set; }
        public DateTime get { get; set; }
        public DateTime ubt { get; set; }
        public DateTime uet { get; set; }
        public int glmt { get; set; }
        public int ct { get; set; }
        public string rmk { get; set; }

        protected override XResp Execute()
        {
            x_ticket d = null;
            if (id > 0) d = db.x_ticket.FirstOrDefault(o => o.ticket_id == id);
            if (d == null) d = new x_ticket() { ctime = DateTime.Now, status = 1 };

            d.topic = topic;
            d.type = type;
            d.count = ct;
            d.getlimit = glmt;
            if (gbt > DateTime.Now.AddYears(-500)) d.get_btime = gbt;
            if (get > DateTime.Now.AddYears(-500)) d.get_etime = get;
            d.max_am = dam;
            d.min_am = xam;
            d.remark = rmk;
            if (ubt > DateTime.Now.AddYears(-500)) d.use_btime = ubt;
            if (uet > DateTime.Now.AddYears(-500)) d.use_etime = uet;
            d.use_limit = umt;
            d.use_range = urg;
            d.getct = d.x_user_ticket.Count();

            if (d.ticket_id == 0)
                db.x_ticket.InsertOnSubmit(d);
            db.SubmitChanges();

            return new XResp() { msg = d.ticket_id + "" };
        }
    }
}
