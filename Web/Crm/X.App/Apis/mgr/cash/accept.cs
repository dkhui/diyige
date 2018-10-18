using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Data;
using X.Web;
using X.Web.Com;
using X.App.Com;

namespace X.App.Apis.mgr.cash
{
    public class accept : xmg
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
            var chg = DB.x_cash_log.FirstOrDefault(o => o.cash_log_id == id);
            if (chg == null) throw new XExcep("T提现记录不存在");
            if (chg.status != 1) throw new XExcep("T提现记录不在待审状态");

            var no = Guid.NewGuid().ToString();
            var psp = Wx.Pay.PayToOpenid(cfg.wx_appid, cfg.wx_mch_id, chg.x_user.wx_opid, no, chg.amount.Value, cfg.wx_certpath, cfg.wx_paykey);
            if (psp.result_code == "SUCCESS")
            {
                chg.status = 2;
                chg.remark = "审核人：" + mg.name + "，审核时间：" + DateTime.Now + "，同意提现，支付成功，微信单号：" + psp.payment_no;

                var v = chg.amount / cfg.credit;
                chg.x_user.x_exp_log.Add(new x_exp_log()
                {
                    ctime = DateTime.Now,
                    remark = "积分兑换支出",
                    val = -v
                });

            }
            else
            {
                chg.remark = "审核人：" + mg.name + "，审核时间：" + DateTime.Now + "，同意提现，支付失败，错误信息：" + psp.err_code_des;
            }

            chg.atime = DateTime.Now;

            SubmitDBChanges();

            return new XResp();
        }
    }
}
