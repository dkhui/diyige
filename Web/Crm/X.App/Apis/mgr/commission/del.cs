using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.commission
{
    public class del : xmg
    {
        public int id { get; set; }

        protected override XResp Execute()
        {
            var ag = db.x_commission.SingleOrDefault(o => o.commission_id == id);
            if (ag == null) throw new XExcep("x0005");

            db.x_commission.DeleteOnSubmit(ag);
            db.SubmitChanges();

            return new XResp();
        }
    }
}
