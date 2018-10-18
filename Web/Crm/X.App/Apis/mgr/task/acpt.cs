using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.task
{
    public class acpt : xmg
    {
        [ParmsAttr(name = "任务ID", min = 1)]
        public long id { get; set; }

        protected override string PowerCode => "E010306";
        protected override XResp Execute()
        {
            var t = db.x_task.FirstOrDefault(o => o.task_id == id);
            if (t == null) throw new XExcep("T任务不存在");

            if (t.status != 1) throw new XExcep("T任务不是待接收状态");
            if (t.mgr_id != mg.mgr_id) throw new XExcep("T非任务接收人不能接收");

            var lg = new x_task_log()
            {
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                remark = "接收了" + db.GetDictName("task.type", t.type) + "任务",
                step = 2
            };

            t.x_task_log.Add(lg);
            t.status = 2;

            //if (t.type == 1) { t.x_project.status = 4; }
            //else if (t.type == 2) t.x_project.status = 5;
            //else if (t.type == 3 || t.type == 4 || t.type == 5) t.x_project.status = 6;

            db.SubmitChanges();

            return new XResp();
        }
    }
}
