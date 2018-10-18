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
    public class setmg : xmg
    {
        [ParmsAttr(name = "项目ID", min = 1)]
        public long id { get; set; }
        [ParmsAttr(name = "设计师ID", min = 1)]
        public long toid { get; set; }

        protected override string PowerCode => "E010217";

        protected override XResp Execute()
        {
            var tmg = db.x_mgr.FirstOrDefault(o => o.mgr_id == toid);
            if (tmg == null) throw new XExcep("T设计师不存在！");

            var p = db.x_project.FirstOrDefault(o => o.project_id == id);

            p.mg_id = tmg.mgr_id;
            p.mg_man = tmg.name;
            p.mg_tel = tmg.tel;

            db.SubmitChanges();

            //发消息给新责任人
            //var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
            //if (!string.IsNullOrEmpty(tmg.wxid))
            //{
            //    Wx.Msg.SendTpl(tk, tmg.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "http://" + cfg.domain + "/pc/index-" + Secret.ToUrlBase64("http://" + cfg.domain + "/pc/task/list-1.html") + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
            //    {
            //        { "first",new Wx.Msg.Tplmsg("你有新的设计任务待接受") },
            //        { "keyword1",new Wx.Msg.Tplmsg(p.no) },
            //        { "keyword2",new Wx.Msg.Tplmsg(p.name)},
            //        { "keyword3", new Wx.Msg.Tplmsg(db.GetDictName("task.type", t.type),"#ff0000") },
            //        { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
            //    });
            //}

            return new XResp();
        }
    }
}
