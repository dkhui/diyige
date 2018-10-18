using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.proj
{
    public class create : xmg
    {
        [ParmsAttr(name = "客户姓名", req = true)]
        public string cname { get; set; }
        [ParmsAttr(name = "客户电话", req = true)]
        public string ctel { get; set; }
        public string wx { get; set; }
        public string qq { get; set; }
        public string ww { get; set; }
        public string shen { get; set; }
        public string shi { get; set; }
        public string xian { get; set; }
        public string addr { get; set; }
        public string src { get; set; }

        protected override string PowerCode => "E010201";

        protected override XResp Execute()
        {
            var u = db.x_user.FirstOrDefault(o => o.tel == ctel);
            if (u == null) u = new x_user() { ctime = DateTime.Now, tel = ctel, headimg = "http://" + cfg.domain + "/img/tx.png" };

            if (db.x_project.Count(o => o.user_id == u.user_id && o.isstop != true && o.status != 8) > 0) throw new XExcep("0x0018");

            u.name = cname;
            u.nickname = cname;
            u.shen = db.GetDictName("sys.city", shen);
            u.shi = db.GetDictName("sys.city", shi);
            u.xian = db.GetDictName("sys.city", xian);
            u.addr = addr;
            u.app = src;

            if (!string.IsNullOrEmpty(wx)) u.wx = wx;
            if (!string.IsNullOrEmpty(qq)) u.qq = qq;
            if (!string.IsNullOrEmpty(ww)) u.ww = ww;

            if (u.user_id == 0) { db.x_user.InsertOnSubmit(u); db.SubmitChanges(); }

            var p = new x_project()
            {
                name = cname + " " + ctel,
                ctime = DateTime.Now,
                user_id = u.user_id,
                rec_addr = db.GetDictName("sys.city", shen) + " " + db.GetDictName("sys.city", shi) + " " + db.GetDictName("sys.city", xian) + " " + addr,
                status = 0,
                isstop = false,
                src = 1,
                ap_id = mg.mgr_id,
                ap_man = mg.name,
                app = src
            };

            db.x_project.InsertOnSubmit(p);
            db.SubmitChanges();

            var od = new x_order()
            {
                ctime = DateTime.Now,
                rec_man = cname,
                rec_tel = ctel,
                status = 0,
                type = 2,
                user_id = u.user_id,
                no = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Tools.GetRandRom(3, 1),
                pid = p.project_id,
                app = src,
                src = 1
            };
            db.x_order.InsertOnSubmit(od);
            db.SubmitChanges();

            return new XResp() { msg = p.project_id + "" };
        }
    }
}
