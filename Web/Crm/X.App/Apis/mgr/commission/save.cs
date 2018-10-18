using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.commission
{
    public class save : xmg
    {
        public int id { get; set; }
        public string frm { get; set; }
        public string cot { get; set; }
        public string name { get; set; }
        public decimal xgpt { get; set; }
        public decimal sopt { get; set; }
        public decimal bupt { get; set; }
        public decimal stpt { get; set; }
        public decimal wtpt { get; set; }
        public decimal elpt { get; set; }
        public DateTime dt { get; set; }
        public string remark { get; set; }

        protected override XResp Execute()
        {
            x_commission e = null;
            if (id > 0)
            {
                e = db.x_commission.SingleOrDefault(o => o.commission_id == id);
                if (e == null) throw new XExcep("0x0005");
            }

            if (e == null)
                e = new x_commission()
                {
                    ctime = DateTime.Now,
                    mg_name = mg.name
                };

            e.frame = frm;
            e.cot = cot;
            e.dq = elpt;
            e.dt = dt;
            e.fa = sopt;
            e.jg = stpt;
            e.jz = bupt;
            e.name = name;
            e.remark = remark;
            e.sl = wtpt;
            e.xg = xgpt;

            if (e.commission_id == 0) db.x_commission.InsertOnSubmit(e);
            db.SubmitChanges();

            return new XResp();
        }
    }
}
