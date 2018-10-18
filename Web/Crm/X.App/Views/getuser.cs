using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;

namespace X.App.Views
{
    public class getuser : xview
    {
        [ParmsAttr(name = "应用ID", req = true)]
        public string appid { get; set; }

        [ParmsAttr(name = "签名", req = true)]
        public string sign { get; set; }
        /// <summary>
        /// 扫码登陆戳
        /// </summary>
        public string sn { get; set; }
        /// <summary>
        /// 回调url
        /// </summary>
        public string bk { get; set; }
        /// <summary>
        /// 分享用户id
        /// </summary>
        public long uid { get; set; }

        protected override string GetParmNames
        {
            get
            {
                //pzWhV49EPgTbSEFj-F0A62A68F289F7DF3601900B5695E23D41AA1E114976096A514B04277A40B6EC-aHR0cDovL2JzLm5ncm9rLjgweGMuY29tL2luZGV4Lmh0bWw=--4d4b87997854dd711f5e69de3ba22000
                return "appid-sign-bk-uid-sn";
            }
        }

        protected override void InitView()
        {
            base.InitView();

            var ap = db.x_app.FirstOrDefault(o => o.appid == appid);
            if (ap == null) throw new XExcep("T应用不存在");

            if (!string.IsNullOrEmpty(bk)) bk = Secret.FromUrlBase64(bk);
            if (string.IsNullOrEmpty(sn))
            {
                if (string.IsNullOrEmpty(bk)) throw new XExcep("T回调地址为空");
                var bsig = Secret.SHA256(appid + bk + ap.appkey);
                if (bsig.ToLower() != sign.ToLower()) throw new XExcep("T数据签名不正确");
            }

            var isWx = Context.Request.UserAgent.Contains("MicroMessenger");
            if (isWx)
            {
                initWx();
            }
            else
            {
                var sn = Secret.MD5(Guid.NewGuid().ToString());
                using (var ms = new MemoryStream())
                {
                    var qr = new QrEncoder();
                    var code = qr.Encode("http://" + cfg.domain + Context.Request.RawUrl + "-" + sn);
                    var rd = new GraphicsRenderer(new FixedModuleSize(15, QuietZoneModules.Two));
                    rd.WriteToStream(code.Matrix, ImageFormat.Jpeg, ms);
                    dict.Add("qrcode", Convert.ToBase64String(ms.GetBuffer()));
                }
                dict["sn"] = sn;
            }
            dict.Add("url", bk);
        }

        void initWx()
        {
            var code = GetReqParms("code");
            if (string.IsNullOrEmpty(code)) toWxUrl("snsapi_base");

            var tk = Wx.Web.GetToken(cfg.wx.appid, cfg.wx.secret, code);
            if (!string.IsNullOrEmpty(tk.errcode)) toWxUrl("snsapi_base");
            var opid = tk.openid;

            var u = db.x_user.FirstOrDefault(o => o.wxid == opid);
            if (u == null) u = new x_user() { ctime = DateTime.Now, wxid = opid, ukey = Secret.MD5(Guid.NewGuid().ToString()) };

            if (u.etime == null || u.etime.Value.AddHours(2) <= DateTime.Now)
            {
                var wxu = Wx.User.GetUserInfo(tk.openid, tk.access_token, true);
                if (!string.IsNullOrEmpty(wxu.errcode)) toWxUrl("snsapi_userinfo");

                u.headimg = wxu.headimgurl;
                u.shen = wxu.province;
                u.shi = wxu.city;
                u.sex = wxu.sex == 1 ? "男" : wxu.sex == 2 ? "女" : "保密";
                u.nickname = wxu.nickname;
                u.etime = DateTime.Now;
            }
            if (u.inviter == 0 || u.inviter == null) u.inviter = uid;
            if (u.user_id == 0) db.x_user.InsertOnSubmit(u);
            db.SubmitChanges();

            dict.Add("iswx", true);
            dict.Add("cu", u);
            dict.Add("opid", opid);

            dict.Add("uk", u.ukey);
            if (!string.IsNullOrEmpty(sn)) CacheHelper.Save("user." + sn, u.ukey);
        }

        protected void toWxUrl(string scope)
        {
            var url = Context.Request.RawUrl.Split('?')[0];
            Context.Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + cfg.wx.appid + "&redirect_uri=" + Context.Server.UrlEncode("http://" + cfg.domain + url.Split('?')[0]) + "&response_type=code&scope=" + scope + "&state=#wechat_redirect");
            Context.Response.End();
        }
    }
}
