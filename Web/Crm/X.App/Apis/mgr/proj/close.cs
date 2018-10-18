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
    public class close : xmg
    {
        [ParmsAttr(name = "项目ID", min = 1)]
        public long id { get; set; }

        protected override string PowerCode => "E010211";
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("T项目不存在");
            if (p.status != 4) throw new XExcep("T项目当前状态不能执行操作！");
            if (p.x_task.Count(o => o.status <= 3 || o.status == 5) > 0) throw new XExcep("T项目有分配的任务没完成，请先完成任务或取消任务");

            var od = db.x_order.FirstOrDefault(o => o.pid == id);
            if (od == null) throw new XExcep("T找不到对应订单");
            if (od.payable > od.paid) throw new XExcep("T订单款项未结清，请先收款");

            p.status = 5;//完成
            od.status = 3;//待发货

            p.x_project_log.Add(new Com.x_project_log()
            {
                cot = "项目已经结项",
                ctime = DateTime.Now,
                type = 8,
                mgr_id = mg.mgr_id
            });

            db.SubmitChanges();

            //发消息给销售部主管角色
            var tmg = db.x_mgr.FirstOrDefault(o => o.dep == 3 && o.role == 7);
            if (tmg != null && !string.IsNullOrEmpty(tmg.wxid))
            {
                var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
                Wx.Msg.SendTpl(tk, tmg.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "http://" + cfg.domain + "/pc/index-" + Secret.ToUrlBase64("http://" + cfg.domain + "/pc/proj/list-3.html") + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("项目设计完成，请按排发货工作") },
                    { "keyword1",new Wx.Msg.Tplmsg(p.no) },
                    { "keyword2",new Wx.Msg.Tplmsg(p.name)},
                    { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                });
            }

            //发消息给客户
            if (!string.IsNullOrEmpty(od.x_user.wxid))
            {
                var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
                Wx.Msg.SendTpl(tk, od.x_user.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "http://m.stbieshu.com/order/detail-" + od.order_id + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("您的定制定单已经设计完成，我们会快发货，请稍待！") },
                    { "keyword1",new Wx.Msg.Tplmsg(p.no) },
                    { "keyword2",new Wx.Msg.Tplmsg(p.name)},
                    { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                });
            }

            return new XResp();
        }
    }
}
