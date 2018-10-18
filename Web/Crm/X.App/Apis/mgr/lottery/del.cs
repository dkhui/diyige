using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery
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
            x_lottery ent = DB.x_lottery.FirstOrDefault(o => o.lottery_id == id);
            if (ent == null) throw new XExcep("T抽奖项目不存在");

            DB.x_lottery_item.DeleteAllOnSubmit(ent.x_lottery_item);
            DB.x_lottery_run.DeleteAllOnSubmit(ent.x_lottery_run);

            DB.x_lottery.DeleteOnSubmit(ent);

            SubmitDBChanges();

            return new XResp();
        }
    }
}
