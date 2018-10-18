using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery.items
{
    public class del : xmg
    {
        public int id { get; set; }

        protected override int powercode
        {
            get
            {
                return 1;
            }
        }
        protected override XResp Execute()
        {
            var ent = DB.x_lottery_item.FirstOrDefault(o => o.lottery_item_id == id);
            if (ent == null) throw new XExcep("T奖品不存在");

            DB.x_lottery_item.DeleteOnSubmit(ent);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
