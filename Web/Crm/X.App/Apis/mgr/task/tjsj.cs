using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.task
{
    public class tjsj : xmg
    {
        [ParmsAttr(name = "任务ID", min = 1)]
        public long id { get; set; }
        protected override XResp Execute()
        {
            var t = db.x_task.FirstOrDefault(o => o.task_id == id);
            if (t == null) throw new XExcep("T任务不存在");

            if (t.status != 1) throw new XExcep("T任务不是待接收状态");

            var lg = new x_task_log()
            {
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                step = 2
            };

            t.x_task_log.Add(lg);
            t.status = 2;

            db.SubmitChanges();

            return new XResp();
        }
    }
}
