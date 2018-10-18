using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Cache;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.order
{
    public class send : xmg
    {
        public int id { get; set; }
        public string ec { get; set; }
        public string en { get; set; }
        public string addr { get; set; }
        public string man { get; set; }
        public string tel { get; set; }
        protected override string PowerCode => "E010406";
        protected override XResp Execute()
        {
            var od = db.x_order.FirstOrDefault(o => o.order_id == id);

            if (od == null) throw new XExcep("0x0015");
            if ((od.type == 1 && od.status != 2) || (od.type == 2 && od.status != 3)) throw new XExcep("0x0037");

            if (od.type == 1) od.status = 3;
            else if (od.type == 2) od.status = 4;

            od.track_com = db.GetDictName("sys.express", ec);
            od.track_no = en;
            od.send_time = DateTime.Now;
            od.send_man = mg.name;
            od.rec_addr = addr;
            od.rec_man = man;
            od.rec_tel = tel;

            db.SubmitChanges();

            if (!string.IsNullOrEmpty(od.x_user.wxid))
            {
                var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
                Wx.Msg.SendTpl(tk, od.x_user.wxid, "BYyLX_59ucVuQiLGqt0EzOPcEhCluAYRhXWP-aj5WVU", "http://m.stbieshu.com/order/detail-" + od.order_id + ".html", new Dictionary<string, Wx.Msg.Tplmsg>() {
                        { "first",new Wx.Msg.Tplmsg("您好，订单已发货") },
                        { "keyword1",new Wx.Msg.Tplmsg(od.no) },
                        { "keyword2",new Wx.Msg.Tplmsg(od.track_com) },
                        { "keyword3",new Wx.Msg.Tplmsg(od.track_no) },
                        { "remark",new Wx.Msg.Tplmsg((od.type==2?"定制编号":"图纸编号")+"："+od.imgno+"，点击查看详情") },
                    });
            }
            else if (!string.IsNullOrEmpty(od.rec_tel))
            {
                Sms.SendPackage(od.rec_tel, od.order_id + "");
            }

            return new XResp();
        }
    }
}
