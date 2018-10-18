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
    public class allot : xmg
    {
        [ParmsAttr(name = "项目ID", min = 1)]
        public long id { get; set; }
        [ParmsAttr(name = "设计师ID", min = 1)]
        public long toid { get; set; }
        public long ismg { get; set; }
        public int type { get; set; }
        public decimal days { get; set; }
        public string desc { get; set; }
        public string remark { get; set; }

        protected override string PowerCode
        {
            get
            {
                if (p == null)
                {
                    p = db.x_project.FirstOrDefault(o => o.project_id == id);
                    if (p == null) throw new XExcep("T项目不存在");
                }
                return p.mg_id == mg.mgr_id ? "#" : "E010209";
            }
        }

        x_project p = null;

        protected override XResp Execute()
        {
            if (p.x_task.Count(o => o.type == type && (o.status == 1 || o.status == 2)) > 0) throw new XExcep("T当前项目已经分配了此类型的任务给设计师，如需重新分配请先取消原分配的任务！");

            var tmg = db.x_mgr.FirstOrDefault(o => o.mgr_id == toid);
            if (tmg == null) throw new XExcep("T设计师不存在！");

            var od = db.x_order.FirstOrDefault(o => o.pid == id);
            if (od == null) throw new XExcep("T找不到对应订单");

            //if (od.status == 1)
            od.status = 2;

            var t = new x_task()
            {
                ctime = DateTime.Now,
                mgr_id = toid,
                type = type,
                opusr = mg.name,
                remark = remark,
                status = 1,
                ptime = DateTime.Now.AddDays((double)days),
                content = desc
            };

            if (ismg == 1)
            {
                p.mg_id = tmg.mgr_id;
                p.mg_man = tmg.name;
                p.mg_tel = tmg.tel;
            }

            //if (p.status == 2) 
            p.status = 3;

            t.name = "项目【" + p.imgno + " " + p.name + "】的" + db.GetDictName("task.type", t.type);
            p.x_task.Add(t);

            var lg = new x_project_log()
            {
                cot = mg.name + "分派了" + t.name + " 任务给" + tmg.name + "，完成期限时间：" + t.ptime?.ToString("yyyy-MM-dd HH:mm"),
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                type = 4
            };
            p.x_project_log.Add(lg);

            var tlg = new x_task_log()
            {
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                remark = "分配了任务",
                step = 1
            };

            t.x_task_log.Add(tlg);

            db.SubmitChanges();

            //发消息给设计师
            var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
            if (!string.IsNullOrEmpty(tmg.wxid))
            {
                Wx.Msg.SendTpl(tk, tmg.wxid, "rttyZNj6JOrUiEjFqwfAgt9S-DiphdRhFB8f2VsczKE", "http://" + cfg.domain + "/pc/index-" + Secret.ToUrlBase64("http://" + cfg.domain + "/pc/task/list-1.html") + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                {
                    { "first",new Wx.Msg.Tplmsg("你有新的设计任务待接受") },
                    { "keyword1",new Wx.Msg.Tplmsg(p.no) },
                    { "keyword2",new Wx.Msg.Tplmsg(p.name)},
                    { "keyword3", new Wx.Msg.Tplmsg(db.GetDictName("task.type", t.type),"#ff0000") },
                    { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                });
            }

            if (p.x_task.Count() == 1)//首次分配通知用户
            {
                if (!string.IsNullOrEmpty(od.x_user.wxid))
                {
                    Wx.Msg.SendTpl(tk, od.x_user.wxid, "5HJHwO5CAdN7ZasvfhFghW5pZddlTxhVDO-6767bGHM", "http://m.stbieshu.com/order/detail-" + od.order_id + ".html", new Dictionary<string, Wx.Msg.Tplmsg>()
                    {
                        { "first",new Wx.Msg.Tplmsg("您的定制单已经开始设计！") },
                        { "keyword1",new Wx.Msg.Tplmsg(od.no) },
                        { "keyword2",new Wx.Msg.Tplmsg(od.ctime?.ToString("yyyy-MM-dd HH:mm"))},
                        { "keyword3", new Wx.Msg.Tplmsg("设计中") },
                        { "remark", new Wx.Msg.Tplmsg("点击查看详情") }
                    });
                }
                else if (!string.IsNullOrEmpty(od.x_user.tel))//短信通知
                { }
            }

            return new XResp();
        }
    }
}
