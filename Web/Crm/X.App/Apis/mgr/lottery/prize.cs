using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.lottery
{
    public class prize : xmg
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

            var q = from lt in DB.x_user_prize
                    select lt;

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.title.Contains(key));

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList().Select(u => new
            {
                name = u.x_user.nickname,
                up = u.x_user.headimg,
                u.title,
                gtime = u.gtime?.ToString("yyyy-MM-dd<br>HH:mm"),
                isget = u.isget == true ? "是" : "否",
                u.prize,
                u.remark,
                u.val,
                ctime = u.ctime?.ToString("yyyy-MM-dd<br>HH:mm")
            }).ToList();
            r.count = q.Count();

            return r;
        }
    }
}
