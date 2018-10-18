using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.secode
{
    /// <summary>
    /// 用户管理列表
    /// </summary>
    public class list : xmg
    {
        public int page { get; set; }
        public int limit { get; set; }
        public string key { get; set; }
        public int bat { get; set; }
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

            var q = from se in DB.x_secode
                    select se;

            if (!string.IsNullOrEmpty(key)) q = q.Where(o => o.outcode == key);
            if (bat > 0) q = q.Where(o => o.batch == bat);

            r.items = q.Skip((page - 1) * limit).Take(limit).ToList().Select(o => new
            {
                id = o.secode_id,
                o.batch,
                icode = o.incode.Value.ToString("00000000"),
                ocode = o.outcode,
                user = getuser(o.user_id),
                stime = o.stime?.Date.ToString("yyyy-MM-dd")
            });

            r.count = q.Count();
            return r;
        }
        string getuser(long? id)
        {
            if (id == null) return "";
            var u = DB.x_user.FirstOrDefault(o => o.user_id == id);
            return u == null ? "" : u.name;
        }
    }
}
