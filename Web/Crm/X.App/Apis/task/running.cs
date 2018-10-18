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
    /// 开奖
    /// 对满足条件的开奖项目进行开奖
    /// </summary>
    public class running : _task
    {
        protected override XResp Execute()
        {
            var rsp = new XResp();
            var lot = DB.x_lottery.FirstOrDefault(o => (o.ltime == null && o.runtp == 1) || o.runtp == 2);
            if (lot == null) return rsp;

            var rules = lot.rules.Split(',');
            var match = 0;
            foreach (var rule in rules)
            {
                var r = rule.Split('-');
                if (r[0] == "1" && (lot.ltime == null || lot.ltime.Value.Date <= DateTime.Now.AddDays(-int.Parse(r[1])).Date)) { match = 1; break; }
                if (r[0] == "2" && cfg.od_count >= int.Parse(r[1])) { match = 2; break; }
                if (r[0] == "3" && cfg.od_amount >= int.Parse(r[1])) { match = 3; break; }
            }

            if (match == 0) return rsp;
            RunLot(lot, match);

            cfg.od_amount = 0;
            cfg.od_count = 0;
            Config.SaveConfig(cfg);

            return rsp;
        }

        void RunLot(x_lottery lot, int m)
        {
            var lg = new x_lottery_run();
            lg.ctime = DateTime.Now;
            lg.open = m + "";
            lg.sum = lot.sum < 1 ? cfg.pool_lott * lot.sum : lot.sum;
            lg.title = lot.topic + DateTime.Now.ToString("yyyy-MM-dd") + "期开奖";
            lot.x_lottery_run.Add(lg);
            SubmitDBChanges();

            var us = new List<long?>();
            foreach (var i in lot.x_lottery_item)//奖项
            {
                if (i.count == 0) continue;
                for (var c = 0; c < i.count; c++)//份数
                {
                    var sc = DB.x_secode
                        .Where(o => !us.Contains(o.user_id) && o.user_id > 0)
                        .Select(o => o.user_id)
                        .OrderBy(o => Guid.NewGuid().ToString())
                        .FirstOrDefault();

                    if (sc == null || sc == 0) continue;

                    var u = DB.x_user.FirstOrDefault(o => o.user_id == sc);
                    if (u == null) continue;

                    var p = new x_user_prize()
                    {
                        isget = false,
                        lottery_run_id = lg.lottery_run_id,
                        prize = i.name,
                        val = i.price < 1 ? lg.sum * i.price : i.price,
                        remark = "",
                        title = lg.title
                    };
                    u.x_user_prize.Add(p);

                    //积分记录
                    if (i.type == 1)
                    {
                        var v = p.val / cfg.credit;
                        var ep = new x_exp_log()
                        {
                            ctime = DateTime.Now,
                            remark = lg.title + "-" + i.name + "-" + v,
                            val = v
                        };
                        u.x_exp_log.Add(ep);
                        u.exp += ep.val;
                    }

                    SubmitDBChanges();
                }
            }
        }
    }
}
