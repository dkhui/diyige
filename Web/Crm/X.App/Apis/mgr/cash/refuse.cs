using System;
using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.cash
{
    public class refuse : xmg
    {
        public int id { get; set; }
        public string reason { get; set; }
        protected override int powercode
        {
            get
            {
                return 1;
            }
        }

        protected override XResp Execute()
        {
            var cit = DB.x_cash_log.FirstOrDefault(o => o.cash_log_id == id);
            if (cit == null) throw new XExcep("T提现记录不存在");
            if (cit.status != 1) throw new XExcep("T提现记录当前不在待审状态");

            cit.status = 3;
            cit.atime = DateTime.Now;
            cit.remark = "申请被拒绝，原因：" + reason + "<br/>审核人：" + mg.name + " " + DateTime.Now;
            cit.x_user.used_exp -= cit.amount / 10;//退回积分
            SubmitDBChanges();

            return new XResp();
        }
    }
}
