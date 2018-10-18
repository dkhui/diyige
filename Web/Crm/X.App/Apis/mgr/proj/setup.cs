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
    public class setup : xmg
    {
        [ParmsAttr(name = "项目ID")]
        public long id { get; set; }
        public string pno { get; set; }
        public string imgs { get; set; }
        public string cot { get; set; }
        public long tkid { get; set; }
        public decimal price { get; set; }
        public string remark { get; set; }

        protected override string PowerCode => "E010208";
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("T项目不存在");

            if (p.status != 1) throw new XExcep("T项目当前状态不能立项");

            var od = db.x_order.FirstOrDefault(o => o.pid == id);
            if (od == null) throw new XExcep("T当前项目有问题，找不到订单");

            if (db.x_project.Count(o => o.no == pno) > 0) throw new XExcep("T项目编号已经存在");
            if (db.x_draw.Count(o => o.no == pno) > 0) throw new XExcep("T图纸编号已经存在");

            od.status = 1;
            od.payable = od.amount = price;
            od.paid = 0;
            od.imgno = pno;

            var lg = new x_project_log()
            {
                cot = "项目立项，编号为：" + pno + "，价格为：" + price,
                ctime = DateTime.Now,
                type = 3,
                mgr_id = mg.mgr_id,
                remark = remark
            };

            p.x_project_log.Add(lg);

            if (tkid > 0)
            {
                var tk = od.x_user.x_user_ticket.FirstOrDefault(o => o.ticket_id == tkid && o.range.Contains("[2]") && o.etime >= DateTime.Now && o.status == 1);
                if (tk != null)
                {
                    od.offam = tk.amount > 1 ? tk.amount : od.amount * (1 - tk.amount);
                    od.offremark = "使用优惠券【" + tk.x_ticket.topic + "】，优惠金额：" + od.offam;
                    od.payable = od.amount - od.offam;
                    od.offtkid = tkid;
                }
                tk.status = 2;
            }

            var dr = new x_draw()
            {
                topic = "定制图纸，编号为：" + pno,
                cate = 2,
                ctime = DateTime.Now,
                depth = p.depth,
                layct = p.layct,
                jarea = p.jarea,
                no = pno,
                no2 = 1,
                open = p.open,
                sort = 99,
                attrs = p.attr,
                @struct = p.frame,
                style = p.style,
                zarea = p.zarea
            };
            db.x_draw.InsertOnSubmit(dr);

            var ls = new List<x_draw.lay>();
            if (p.attr.Contains("[6]")) ls.Add(new x_draw.lay() { height = 3, id = 11, pic = "", text = "" });
            for (var i = 1; i <= p.layct; i++) ls.Add(new x_draw.lay() { height = 3, id = i, pic = "", text = "" });
            //if (p.attr.Contains("[5]")) ls.Add(new x_draw.lay() { height = 3, id = 11, pic = "", text = "" });

            dr.layers = Serialize.ToJson(ls);

            p.contract_no = pno;
            p.imgno = pno + "_1";
            p.cot = cot;
            p.no = pno;
            p.status = 2;
            db.SubmitChanges();

            //发消息给设计部主管角色
            var wtk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
            var tmg = db.x_mgr.FirstOrDefault(o => o.dep == 6 && o.role == 2);
            if (tmg != null && !string.IsNullOrEmpty(tmg.wxid))
            {
                Wx.Msg.SendTpl(wtk, tmg.wxid, "5HJHwO5CAdN7ZasvfhFghW5pZddlTxhVDO-6767bGHM", "http://" + cfg.domain + "/pc/index-" + Secret.ToUrlBase64("http://" + cfg.domain + "/pc/proj/list-2.html") + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("客户【"+p.name+"】已经立项，请安排设计") },
                    { "keyword1",new Wx.Msg.Tplmsg(p.no) },
                    { "keyword2",new Wx.Msg.Tplmsg(p.ctime?.ToString("yyyy-MM-dd HH:mm"))},
                    { "keyword3", new Wx.Msg.Tplmsg("已立项") },
                    { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                });
            }

            //发消息给额头用户
            if (!string.IsNullOrEmpty(od.x_user.wxid))
            {
                Wx.Msg.SendTpl(wtk, od.x_user.wxid, "5HJHwO5CAdN7ZasvfhFghW5pZddlTxhVDO-6767bGHM", "http://m.stbieshu.com/order/detail-" + od.order_id + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("您的定制单已经立项，我们会快安排设计！") },
                    { "keyword1",new Wx.Msg.Tplmsg(od.no) },
                    { "keyword2",new Wx.Msg.Tplmsg(od.ctime?.ToString("yyyy-MM-dd HH:mm"))},
                    { "keyword3", new Wx.Msg.Tplmsg("已立项") },
                    { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                });
            }
            else if (!string.IsNullOrEmpty(od.x_user.tel))//发送短信
            {

            }

            return new XResp();
        }
    }
}
