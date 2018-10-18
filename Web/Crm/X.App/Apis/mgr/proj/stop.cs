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
    public class stop : xmg
    {
        [ParmsAttr(name = "项目ID", min = 1)]
        public long id { get; set; }
        protected override string PowerCode => "E010211";
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("T项目不存在");
            if (p.isstop == true) throw new XExcep("T项目已经是中止状态！");

            if (p.x_task.Count(o => o.status <= 3 || o.status == 5) > 0) throw new XExcep("T项目还有未处理的任务，请先处理任务！");

            p.isstop = true;

            p.x_project_log.Add(new Com.x_project_log()
            {
                cot = "项目中止",
                ctime = DateTime.Now,
                type = 10,
                mgr_id = mg.mgr_id
            });

            db.SubmitChanges();

            //通知设计师
            var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
            foreach (var t in p.x_task)
            {
                if (string.IsNullOrEmpty(t.x_mgr.wxid)) continue;
                Wx.Msg.SendTpl(tk, t.x_mgr.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("项目中止，请知悉","#ff0000") },
                    { "keyword1",new Wx.Msg.Tplmsg(p.no) },
                    { "keyword2",new Wx.Msg.Tplmsg(p.name)},
                    { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                });
            }
            //通知销售
            var tmg = db.x_mgr.FirstOrDefault(o => o.mgr_id == p.ap_id);
            if (tmg != null && !string.IsNullOrEmpty(tmg.wxid))
            {
                Wx.Msg.SendTpl(tk, tmg.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("项目中止","#ff0000") },
                    { "keyword1",new Wx.Msg.Tplmsg(p.no) },
                    { "keyword2",new Wx.Msg.Tplmsg(p.name)},
                    { "remark",new Wx.Msg.Tplmsg("请暂停相关设计制作任务")}
                });
            }

            return new XResp();
        }
    }
}
