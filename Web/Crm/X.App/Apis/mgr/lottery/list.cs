using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery
{
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }

        protected override int powercode
        {
            get
            {
                return 1;
            }
        }

        protected override XResp Execute()
        {
            var r = new Resp_List();
            r.page = page;

            var q = from lt in DB.x_lottery
                    select lt;

            //if (city > 0) q = q.Where(o => o.city == (mg.x_role.power == "###" ? city : mg.city));
            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.topic.Contains(key));

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList().Select(u => new
            {
                id = u.lottery_id,
                u.topic,
                u.remark,
                sum = u.sum < 1 ? u.sum?.ToString("F3") + "%" : u.sum?.ToString("F2"),
                ctime = u.ctime.Value.ToString("yyyy-MM-dd<br>HH:mm"),
                ltime = u.ltime?.ToString("yyyy-MM-dd<br>HH:mm:ss"),
                status = getstatus(u.status),
                tp = u.runtp == 1 ? "即时开奖" : getrules(u.rules),
            }).ToList();
            r.count = q.Count();

            return r;
        }

        string getrules(string rs)
        {
            if (string.IsNullOrEmpty(rs)) return "";
            var ru = "";
            foreach (var r in rs.Split(','))
            {
                var _r = r.Split('-');
                if (_r[0] == "1") ru += "每隔" + _r[1] + "分钟<br/>";
                else if (_r[0] == "2") ru += "订单数每满" + _r[1] + "笔<br/>";
                else if (_r[0] == "3") ru += "订单额每满" + _r[1] + "元<br/>";
            }
            return ru;
        }

        string getstatus(int? st)
        {
            if (st == null) return "未知";
            else if (st == 1) return "待开奖";
            else if (st == 2) return "已开奖";
            else return "未知";
        }

    }
}
