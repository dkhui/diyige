using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery
{
    public class save : xmg
    {
        public int id { get; set; }
        public string topic { get; set; }
        public int tp { get; set; }
        public decimal sum { get; set; }
        public string tj { get; set; }
        public int hc { get; set; }
        public int oc { get; set; }
        public int ac { get; set; }
        public string desc { get; set; }

        protected override int powercode
        {
            get
            {
                return 1;
            }
        }
        protected override XResp Execute()
        {
            x_lottery ent = null;
            if (id > 0)
            {
                ent = DB.x_lottery.SingleOrDefault(o => o.lottery_id == id);
                if (ent == null) throw new XExcep("0x0005");
            }
            if (ent == null) ent = new x_lottery() { ctime = DateTime.Now };

            ent.remark = desc;
            ent.sum = sum;
            ent.runtp = tp;
            ent.topic = topic;
            ent.status = 1;
            if (tp == 2)
            {
                ent.rules = "";
                if (tj.Contains("[1]")) ent.rules += "1-" + hc + ",";
                if (tj.Contains("[2]")) ent.rules += "2-" + oc + ",";
                if (tj.Contains("[3]")) ent.rules += "3-" + ac + ",";
                ent.rules = ent.rules.TrimEnd(',');
                if (string.IsNullOrEmpty(ent.rules)) throw new XExcep("T请选择至少一个条件");
            }

            if (id == 0) DB.x_lottery.InsertOnSubmit(ent);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
