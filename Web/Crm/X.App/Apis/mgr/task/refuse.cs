using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.task
{
    public class refuse : xmg
    {
        [ParmsAttr(name = "任务ID", min = 1)]
        public long id { get; set; }
        protected override string PowerCode => "E010311";
        protected override XResp Execute()
        {
            var t = db.x_task.FirstOrDefault(o => o.task_id == id);
            if (t == null) throw new XExcep("T任务不存在");

            if (t.status != 1) throw new XExcep("T任务不是待接收状态");
            if (t.mgr_id != mg.mgr_id) throw new XExcep("T非任务接收人不能拒收");

            var lg = new x_task_log()
            {
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                remark = "拒收了" + db.GetDictName("task.type", t.type) + "任务",
                step = 2
            };

            t.x_task_log.Add(lg);
            t.status = 7;

            db.SubmitChanges();

            return new XResp();
        }
    }
}
