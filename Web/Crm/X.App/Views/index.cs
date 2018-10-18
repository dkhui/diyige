using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Cache;
using X.Core.Utility;
using X.Web;
using X.Web.Com;
using X.Web.Views;

namespace X.App.Views
{
    public class index : xview
    {
        protected override void InitView()
        {
            base.InitView();


        }

        public override byte[] GetResponse()
        {
            return base.GetResponse();
            //InitView();
            //var c = new DefaultAopClient(cfg.pay.ali.gateway, cfg.pay.ali.appid, cfg.pay.ali.mch_key, false);
            ////var p = new
            ////{
            ////    body = "Vip购买",
            ////    subject = "Vip购买",
            ////    productcode = "QUICK_MSECURITY_PAY",
            ////    out_trade_no = "1122558745484578",
            ////    total_amount = 0.01,
            ////    timeoutexpress = "30m"
            ////};
            //AlipayTradeAppPayModel model = new AlipayTradeAppPayModel();
            ////model.Body = ("一个月会员");
            //model.Subject = ("一个月会员");
            //model.OutTradeNo = ("1122558745484578");
            //model.TimeoutExpress = ("30m");
            //model.TotalAmount = "0.01";
            ////model.ProductCode = ("QUICK_MSECURITY_PAY");

            //var req = new AlipayTradeAppPayRequest();
            //req.SetBizModel(model);

            //req.SetNotifyUrl("http://" + cfg.domain + "/notify-2-1122558745484578.html");
            //var rsp = c.SdkExecute(req);
            //if (rsp.IsError) throw new XExcep("0x0024", rsp.Msg + "<br/>" + rsp.SubMsg);

            //return Encoding.UTF8.GetBytes(rsp.Body);
        }

        class back : XResp
        {
            public string appid { get; set; }
            public string amount { get; set; }
            public string timestramp { get; set; }
            public string nonce_str { get; set; }
            public string package { get; set; }
            public string sign { get; set; }
            public string qrcode { get; set; }
            public string mweburl { get; set; }
        }
    }
}
