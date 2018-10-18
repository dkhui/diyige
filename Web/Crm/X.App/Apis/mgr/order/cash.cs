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
    public class cash : xmg
    {
        public int id { get; set; }
        /// <summary>
        /// 回款金额
        /// </summary>
        public int type { get; set; }
        public decimal am { get; set; }
        /// <summary>
        /// 1、微信
        /// 2、支付宝
        /// 3、银行
        /// </summary>
        public int way { get; set; }
        /// <summary>
        /// 回款时间
        /// </summary>
        public string btime { get; set; }
        /// <summary>
        /// 发票
        /// </summary>
        public string tk { get; set; }
        public string remark { get; set; }

        protected override string PowerCode => "E010405";
        protected override XResp Execute()
        {
            var od = db.x_order.FirstOrDefault(o => o.order_id == id);

            if (od == null) throw new XExcep("0x0015");
            //if (am > od.payable - od.paid) throw new XExcep("0x0037");

            var pb = new x_order_pay()
            {
                amount = am,
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                way = way,
                ticket = tk,
                type = type,
                btime = DateTime.Parse(btime),
                remark = remark
            };
            od.x_order_pay.Add(pb);

            od.paid += am;
            if (od.paid >= od.payable && od.type == 1) od.status = 2;

            var p = db.x_project.FirstOrDefault(o => o.project_id == od.pid);
            if (od.type == 2 && od.pid > 0)
            {
                var lg = new x_project_log()
                {
                    ctime = DateTime.Now,
                    mgr_id = mg.mgr_id,
                };
                if (type == 1) { lg.cot = "收到项目全款：" + am; lg.type = 12; }
                if (type == 2) { lg.cot = "收到项目定金：" + am; lg.type = 2; }
                else if (type == 3) { lg.cot = "收到项目方案确认款项：" + am; lg.type = 5; }
                else if (type == 4) { lg.cot = "收到项目效果确认款项（尾款）：" + am; lg.type = 7; }
                else if (type == 5) { lg.cot = "收到项目修改款：" + am; lg.type = 13; }
                p.x_project_log.Add(lg);
            }

            var wtk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
            //发消息给项目负责人
            var tmg = db.x_mgr.FirstOrDefault(o => o.mgr_id == p.mg_id);
            if (tmg != null && !string.IsNullOrEmpty(tmg.wxid))
            {
                Wx.Msg.SendTpl(wtk, tmg.wxid, "DiOt1QFEQhQpAjqiCkNRuxx2Qyi1NyS9wPLr4qe__rc", "", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("收到客户"+pb.tname+"款项，项目："+p.no+"，客户："+p.name) },
                    { "keyword1",new Wx.Msg.Tplmsg(pb.amount?.ToString("F2")) },
                    { "keyword2",new Wx.Msg.Tplmsg(pb.wname)},
                    { "keyword3", new Wx.Msg.Tplmsg(btime) },
                    { "keyword4", new Wx.Msg.Tplmsg(od.no) },
                    { "remark", new Wx.Msg.Tplmsg("请继续完成设计任务") }
                });
            }

            //发消息给用户
            if (!string.IsNullOrEmpty(od.x_user.wxid))
            {
                Wx.Msg.SendTpl(wtk, od.x_user.wxid, "DiOt1QFEQhQpAjqiCkNRuxx2Qyi1NyS9wPLr4qe__rc", "", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("已经收到您的"+pb.tname+"付款") },
                    { "keyword1",new Wx.Msg.Tplmsg(pb.amount?.ToString("F2")) },
                    { "keyword2",new Wx.Msg.Tplmsg(pb.wname)},
                    { "keyword3", new Wx.Msg.Tplmsg(btime) },
                    { "keyword4", new Wx.Msg.Tplmsg(od.no) },
                    { "remark",new Wx.Msg.Tplmsg("谢谢支持！")}
                });
            }

            db.SubmitChanges();
            return new XResp();
        }
    }
}
