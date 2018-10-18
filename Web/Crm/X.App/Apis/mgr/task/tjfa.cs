using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.task
{
    public class tjfa : xmg
    {
        [ParmsAttr(name = "任务ID", min = 1)]
        public long id { get; set; }
        public string imgs { get; set; }

        protected override XResp Execute()
        {
            var t = db.x_task.FirstOrDefault(o => o.task_id == id);
            if (t == null) throw new XExcep("T任务不存在");

            if (t.status != 2) throw new XExcep("T任务不是待提交状态");




            return new XResp();
        }
    }
}
