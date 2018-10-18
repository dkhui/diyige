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
    public abstract class sub : xmg
    {
        [ParmsAttr(name = "任务ID", min = 1)]
        public long id { get; set; }

        [ParmsAttr(name = "源文件", req = true)]
        public string src { get; set; }

        [ParmsAttr(name = "文件内容")]
        public string cot { get; set; }

        [ParmsAttr(name = "打印文件")]
        public string pdfs { get; set; }
        /// <summary>
        /// 仅保存
        /// </summary>
        public int sv { get; set; }

        protected x_task t = null;
        protected x_draw dr = null;

        protected override string PowerCode => "E010307";

        protected abstract void Submit();

        protected override XResp Execute()
        {
            t = db.x_task.FirstOrDefault(o => o.task_id == id);
            if (t == null) throw new XExcep("T任务不存在");
            if (t.status != 2 && t.status != 5) throw new XExcep("T任务当前状态不能提交任务");
            if (t.x_project.status != 3) throw new XExcep("T项目当前状态不能提交任务");

            dr = db.x_draw.FirstOrDefault(o => o.no == t.x_project.no);
            if (dr == null) throw new XExcep("T找不到对应图纸");

            var f = dr.x_draw_file.FirstOrDefault(o => o.type == t.type);
            if (f == null) { f = new x_draw_file() { ctime = DateTime.Now, type = t.type, mgr_id = mg.mgr_id }; dr.x_draw_file.Add(f); }
            f.file = src;
            f.prints = pdfs;
            f.name = db.GetDictName("task.type", t.type);

            Submit();

            t.x_task_log.Add(new x_task_log()
            {
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                step = 3,
                remark = "提交了任务"
            });

            if (sv != 1) t.status = 3;

            db.SubmitChanges();

            //发消息给项目负责人
            var tmg = db.x_mgr.FirstOrDefault(o => o.mgr_id == t.x_project.mg_id);
            if (tmg != null && !string.IsNullOrEmpty(tmg.wxid))
            {
                var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
                Wx.Msg.SendTpl(tk, tmg.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "http://" + cfg.domain + "/pc/index-" + Secret.ToUrlBase64("http://" + cfg.domain + "/pc/task/list-2.html") + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("任务完成已经提交，请审核") },
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
