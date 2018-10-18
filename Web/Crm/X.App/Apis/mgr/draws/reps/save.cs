using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws.reps
{
    public class save : xmg
    {
        public int id { get; set; }
        public string cot { get; set; }
        protected override string PowerCode => "E01050901";
        protected override XResp Execute()
        {
            var rp = db.x_draw_reply.FirstOrDefault(o => o.draw_reply_id == id);
            if (rp == null) throw new XExcep("0x0036");

            rp.rep = cot;
            rp.reper = mg.name;
            rp.rtime = DateTime.Now;

            var u = db.x_user.FirstOrDefault(o => o.user_id == rp.user_id);
            if (u != null)
            {
                if (!string.IsNullOrEmpty(u.wxid))
                {
                    var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
                    Wx.Msg.SendTpl(tk, u.wxid, "YnYsgMKjvzTQpPsatNcAykJ_rz1Aj3IlwiokeoW_M48", "http://m.stbieshu.com/detail-" + rp.draw_id + ".html#ask", new Dictionary<string, Wx.Msg.Tplmsg>() {
                        { "first",new Wx.Msg.Tplmsg("您好，您收到一条回复") },
                        { "keyword1",new Wx.Msg.Tplmsg(mg.name+" "+mg.tel) },
                        { "keyword2",new Wx.Msg.Tplmsg(DateTime.Now.ToString("yyyy-MM-dd HH:mm")) },
                        { "keyword3",new Wx.Msg.Tplmsg(cot) },
                        { "remark",new Wx.Msg.Tplmsg("专属客服已经对你的提问进行了回复，点击查看详情") },
                    });
                }
                else if (!string.IsNullOrEmpty(u.tel))
                {
                    Sms.DrawReply(u.tel, rp.draw_id + "");
                }
            }

            //发消息给销售部主管角色
            //var tmg = db.x_mgr.FirstOrDefault(o => o.dep == 3 && o.role == 7);
            //if (tmg != null && !string.IsNullOrEmpty(tmg.wxid))
            //{
            //    var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
            //    Wx.Msg.SendTpl(tk, tmg.wxid, "a2nO9mbTEU2L21Dh65zZHuZTlQRJBYOj9UvGJOf-Xfw", "http://" + cfg.domain + "/pc/index-" + Secret.ToBase64("http://" + cfg.domain + "/pc/proj/list-1.html") + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
            //    {
            //        { "first",new Wx.Msg.Tplmsg("收到新的定制订单，情尽快联系客户") },
            //        { "tradeDateTime",new Wx.Msg.Tplmsg(od.ctime?.ToString("yyyy-MM-dd HH:mm")) },
            //        { "orderType",new Wx.Msg.Tplmsg("定制定单","#ff0000")},
            //        { "customerInfo", new Wx.Msg.Tplmsg(cu.name) },
            //        { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
            //    });
            //}

            db.SubmitChanges();

            return new XResp();
        }
    }
}
