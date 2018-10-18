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
    public class get : xmg
    {
        public string tel { get; set; }
        public long id { get; set; }

        protected override XResp Execute()
        {
            var tc = db.x_ticket.FirstOrDefault(o => o.ticket_id == id && o.status == 1 && o.type == 2);
            if (tc == null) throw new XExcep("0x0026");

            if (tc.x_user_ticket.Count() >= tc.count) throw new XExcep("0x0028");

            if (!string.IsNullOrEmpty(tel))
            {
                var ct = tc.x_user_ticket.Count(o => o.tel == tel);
                if (ct >= tc.getlimit) throw new XExcep("0x0029");
            }

            var ut = new x_user_ticket()
            {
                amount = Tools.GetRandNext((int)(tc.min_am * 10000), (int)(tc.max_am * 10000)) / (decimal)10000.0,
                ctime = DateTime.Now,
                tel = tel,
                etime = tc.use_etime,
                no = tc.ticket_id.ToString("0000") + " " + (tc.x_user_ticket.Count() + 1).ToString("0000") + " " + Tools.GetRandRom(4),
                limit = tc.use_limit,
                range = tc.use_range,
                status = 4,
                remark = mg.name + "给用户" + tel + "生成了此券"
            };

            tc.x_user_ticket.Add(ut);
            tc.getct = tc.x_user_ticket.Count();

            db.SubmitChanges();
            return new XResp() { msg = ut.user_ticket_id + "" };
        }
    }
}
