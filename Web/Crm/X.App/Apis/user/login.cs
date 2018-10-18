using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.user
{
    public class login : xapi
    {
        public string tel { get; set; }
        public string code { get; set; }
        public string sn { get; set; }
        public string name { get; set; }
        public string headimg { get; set; }
        public string shen { get; set; }
        public string shi { get; set; }
        public string sex { get; set; }
        public string opid { get; set; }
        protected override XResp Execute()
        {
            var uk = "";
            if (!string.IsNullOrEmpty(sn))
            {
                uk = CacheHelper.Get<string>("user." + sn);
                if (!string.IsNullOrEmpty(uk))
                    return new XResp() { msg = uk };
            }

            if (!string.IsNullOrEmpty(tel) && !string.IsNullOrEmpty(code))
            {
                var cd = CacheHelper.Get<string>("sms.code." + tel);
                if (code != cd) throw new XExcep("0x0031");

                var u = db.x_user.FirstOrDefault(o => o.tel == tel);
                if (u == null) u = new Com.x_user() { ctime = DateTime.Now, tel = tel, nickname = "建房好友", headimg = "http://" + cfg.domain + "/img/tx.png" };

                if (!string.IsNullOrEmpty(name)) { u.name = u.nickname = name; }
                if (!string.IsNullOrEmpty(headimg)) u.headimg = headimg;
                if (!string.IsNullOrEmpty(shen)) u.shen = shen;
                if (!string.IsNullOrEmpty(shi)) u.shi = shi;
                if (!string.IsNullOrEmpty(sex)) u.sex = sex;
                if (!string.IsNullOrEmpty(opid) && string.IsNullOrEmpty(u.wxid)) u.wxid = opid;

                if (u.etime == null || u.etime?.AddHours(2) < DateTime.Now)
                {
                    u.ukey = Secret.MD5(Guid.NewGuid().ToString());
                    u.etime = DateTime.Now;
                }

                if (u.user_id == 0) db.x_user.InsertOnSubmit(u);
                db.SubmitChanges();

                if (!string.IsNullOrEmpty(sn)) CacheHelper.Save("user." + sn, u.ukey, 30);
                else uk = u.ukey;
            }
            return new XResp() { msg = uk };
        }
    }
}
