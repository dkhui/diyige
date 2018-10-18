using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery.items
{
    public class save : xmg
    {
        public int id { get; set; }

        [ParmsAttr(name = "抽奖项目id", min = 1)]
        public int lot_id { get; set; }

        [ParmsAttr(name = "奖品名称", req = true)]
        public string name { get; set; }
        public int type { get; set; }
        public int count { get; set; }
        public decimal price { get; set; }
        public decimal chance { get; set; }
        public string pic { get; set; }
        public string remark { get; set; }

        protected override int powercode
        {
            get
            {
                return 1;
            }
        }
        protected override XResp Execute()
        {
            x_lottery_item ent = null;
            if (id > 0)
            {
                ent = DB.x_lottery_item.SingleOrDefault(o => o.lottery_item_id == id);
                if (ent == null) throw new XExcep("0x0005");
            }
            if (ent == null) ent = new x_lottery_item() { lottery_id = lot_id };

            ent.name = name;
            ent.type = type;
            ent.price = price;
            ent.count = count;
            ent.chance = chance;
            ent.pic = pic;
            ent.remark = remark;

            if (id == 0) DB.x_lottery_item.InsertOnSubmit(ent);
            SubmitDBChanges();

            return new XResp();
        }
    }
}
