using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.task
{
    public class pass : xmg
    {
        [ParmsAttr(name = "任务ID", min = 1)]
        public int id { get; set; }
        protected override string PowerCode => "E010308";
        protected override XResp Execute()
        {
            var t = db.x_task.FirstOrDefault(o => o.task_id == id);
            if (t == null) throw new XExcep("T任务不存在");
            if (t.status != 3) throw new XExcep("T任务状态不能执行此操作");

            t.x_task_log.Add(new Com.x_task_log()
            {
                ctime = DateTime.Now,
                step = 5,
                mgr_id = mg.mgr_id,
                remark = db.GetDictName("task.type", t.type) + "任务审核通过"
            });

            t.status = 4;
            t.etime = DateTime.Now;

            var yct = db.GetDictList("task.type", (object)t.x_project.cot).Count();
            var ct = t.x_project.x_task.Where(o => o.status == 4).GroupBy(o => o.type).Count();
            if (ct == yct) t.x_project.status = 4;//待结项

            db.SubmitChanges();

            var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
            var m1 = "";//给管理员
            var m2 = "";//给客户
            switch (t.type)
            {
                case 1:
                    m1 = "方案设计已经完成，请联系客户确认并回收款项！";
                    m2 = "方案设计已经完成，客服很快将联系您确认";
                    break;
                case 2:
                    m1 = "效果设计已经完成，请联系客户确认并回收款项！";
                    m2 = "效果设计已经完成，客服很快将联系您确认";
                    break;
            }
            //发给销售
            if (!string.IsNullOrEmpty(m1))
            {
                var tmg = db.x_mgr.FirstOrDefault(o => o.mgr_id == t.x_project.ap_id);
                if (tmg != null && !string.IsNullOrEmpty(tmg.wxid))
                {
                    Wx.Msg.SendTpl(tk, tmg.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "http://" + cfg.domain + "/pc/index-" + Secret.ToUrlBase64("http://" + cfg.domain + "/pc/proj/list-3.html") + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                    {
                        { "first",new Wx.Msg.Tplmsg(m1) },
                        { "keyword1",new Wx.Msg.Tplmsg(t.x_project.no) },
                        { "keyword2",new Wx.Msg.Tplmsg(t.x_project.name)},
                        { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                    });
                }
            }
            if (!string.IsNullOrEmpty(m2))
            {
                var tus = db.x_user.FirstOrDefault(o => o.user_id == t.x_project.user_id);
                if (tus != null && !string.IsNullOrEmpty(tus.wxid))
                {
                    Wx.Msg.SendTpl(tk, tus.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "", new Dictionary<string, Wx.Msg.Tplmsg>()
                    {
                        { "first",new Wx.Msg.Tplmsg(m2) },
                        { "keyword1",new Wx.Msg.Tplmsg(t.x_project.no) },
                        { "keyword2",new Wx.Msg.Tplmsg(t.x_project.name)},
                        { "remark",new Wx.Msg.Tplmsg("请保持手机畅通")}
                    });
                }
            }

            return new XResp();
        }
    }
}
