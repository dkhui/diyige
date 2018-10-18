using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.user
{
    /// <summary>
    /// 用户管理列表
    /// </summary>
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }
        public int tp { get; set; }

        protected override XResp Execute()
        {
            var r = new XList();
            r.page = page;

            var q = from u in db.x_user
                    join iu in db.x_user.Select(o => new { o.user_id, o.name, o.nickname, o.headimg, o.tel }) on u.inviter equals iu.user_id into us
                    from o in us.DefaultIfEmpty()
                    select new
                    {
                        id = u.user_id,
                        name = u.name,
                        u.tel,
                        addr = u.shen + " " + u.shi + " " + u.xian + " " + u.addr,
                        u.nickname,
                        u.headimg,
                        u.ctime,
                        u.etime,
                        invt = us.FirstOrDefault()  //u.inviter > 0 ? db.x_user.Where(o => o.user_id == u.inviter).Select(o => new { o.name, o.nickname, o.headimg, o.tel }).FirstOrDefault() : null
                    };

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.name.Contains(key) || o.tel.Contains(key) || o.nickname.Contains(key));

            var list = q.OrderByDescending(o => o.ctime).Skip((page - 1) * limit).Take(limit).ToList();

            r.items = list.Select(o => new
            {
                o.addr,
                ctime = o.ctime?.ToString("yyyy-MM-dd HH:mm"),
                etime = o.etime?.ToString("yyyy-MM-dd HH:mm"),
                o.headimg,
                o.id,
                o.invt,
                o.name,
                o.nickname,
                o.tel
            }).ToList();
            r.count = q.Count();

            return r;
        }

    }
}
