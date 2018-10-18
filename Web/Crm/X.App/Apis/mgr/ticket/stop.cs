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
    public class stop : xmg
    {
        public int id { get; set; }

        protected override XResp Execute()
        {
            x_ticket d = null;
            if (id > 0) d = db.x_ticket.FirstOrDefault(o => o.ticket_id == id);
            if (d == null) throw new XExcep("0x0026");

            d.status = 3 - d.status;

            db.SubmitChanges();

            return new XResp() { msg = d.ticket_id + "" };
        }
    }
}
