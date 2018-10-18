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
    public class unpass : xmg
    {
        [ParmsAttr(name = "任务ID", min = 1)]
        public int id { get; set; }
        [ParmsAttr(name = "原因", req = true)]
        public string cot { get; set; }

        protected override string PowerCode => "E010308";
        protected override XResp Execute()
        {
            var t = db.x_task.FirstOrDefault(o => o.task_id == id);
            if (t == null) throw new XExcep("T任务不存在");
            if (t.status != 3) throw new XExcep("T任务状态不能执行此操作");

            t.x_task_log.Add(new Com.x_task_log()
            {
                ctime = DateTime.Now,
                step = 4,
                mgr_id = mg.mgr_id,
                remark = db.GetDictName("task.type", t.type) + "任务审核不通过，原因：" + cot + "，发回修改"
            });

            t.status = 5;

            db.SubmitChanges();

            //发消息给设计师
            if (!string.IsNullOrEmpty(t.x_mgr.wxid))
            {
                var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
                Wx.Msg.SendTpl(tk, t.x_mgr.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "http://" + cfg.domain + "/pc/index-" + Secret.ToUrlBase64("http://" + cfg.domain + "/pc/task/list-2.html") + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("任务审核不通过，请修改后重新提交") },
                    { "keyword1",new Wx.Msg.Tplmsg(t.x_project.no) },
                    { "keyword2",new Wx.Msg.Tplmsg(t.x_project.name)},
                    { "keyword3", new Wx.Msg.Tplmsg(db.GetDictName("task.type", t.type),"#ff0000") },
                    { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                });
            }

            return new XResp();
        }
    }
}
