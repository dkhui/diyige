using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Data;
using X.Web.Com;

namespace X.App.Apis.task
{
    /// <summary>
    /// 订单结算
    /// 只操作超过退货时间的订单
    /// </summary>
    public class closing : _task
    {
        protected override XResp Execute()
        {
            var r = new XResp();
            var ods = DB.x_order.Where(o => o.iscancel == false && o.isclosing == false && o.isrefund == false && o.status == 4 && o.sign_time >= DateTime.Now.AddDays(-cfg.refdays)).Take(5);
            if (ods.Count() == 0) return r;

            foreach (var o in ods)
            {
                o.isclosing = true;
                allotBonus(o.x_user.invter, 1, o.yf_amount.Value);

                var log = new x_lottery_log()
                {
                    amount = o.yf_amount.Value * cfg.brate,
                    ctime = DateTime.Now,
                    oamount = o.yf_amount,
                    rate = cfg.brate
                };

                o.x_lottery_log.Add(log);

                cfg.pool_lott += o.yf_amount.Value * cfg.brate;
                cfg.pool_fund += o.yf_amount.Value * cfg.mrate;
                cfg.od_count++;
                cfg.od_amount += o.yf_amount.Value;
            }

            Config.SaveConfig(cfg);
            SubmitDBChanges();
            return new XResp();
        }

        void allotBonus(long? uid, int lv, decimal amount)
        {
            if (lv > 10 || uid == null || uid == 0) return;
            var u = DB.x_user.FirstOrDefault(o => o.user_id == uid);
            if (u == null) return;
            var v = amount * cfg.lv_cent[lv - 1] / 10;
            var l = new x_exp_log()
            {
                ctime = DateTime.Now,
                remark = "您的第" + lv + "级用户【" + u.nickname + "】成交分到：" + v + "积分",
                val = v
            };
            u.x_exp_log.Add(l);
            allotBonus(u.invter, lv + 1, amount);
        }
    }
}
