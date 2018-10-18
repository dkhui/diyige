using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.ticket.gets
{
    public class del : xmg
    {

        [ParmsAttr(name = "项目ID")]
        public int id { get; set; }
        protected override XResp Execute()
        {
            var p = db.x_user_ticket.FirstOrDefault(o => o.user_ticket_id == id);
            if (p == null) throw new XExcep("0x0026");

            db.x_user_ticket.DeleteOnSubmit(p);

            db.SubmitChanges();

            return new XResp();
        }
    }
}
