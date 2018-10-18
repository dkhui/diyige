using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.order
{
    public class acpt : xmg
    {
        [ParmsAttr(name = "订单ID", min = 1)]
        public long id { get; set; }
        protected override string PowerCode => "E010407";

        protected override XResp Execute()
        {
            var od = db.x_order.FirstOrDefault(o => o.order_id == id);

            if (od == null) throw new XExcep("0x0015");
            if ((od.type == 1 && od.status != 3) || (od.type == 2 && od.status != 4)) throw new XExcep("0x0016");
            if (od.paid < od.payable) throw new XExcep("0x0017");

            if (od.type == 1) od.status = 4;
            else if (od.type == 2) od.status = 5;

            db.SubmitChanges();

            return new XResp();
        }
    }
}
