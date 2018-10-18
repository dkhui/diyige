using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.task
{
    public class cancel : xmg
    {
        public int id { get; set; }
        protected override string PowerCode => "E010309";
        protected override XResp Execute()
        {
            var t = db.x_task.FirstOrDefault(o => o.task_id == id);
            if (t == null) throw new XExcep("T任务不存在");
            if (t.status > 2) throw new XExcep("T任务状态不能执行此操作");

            t.x_task_log.Add(new Com.x_task_log()
            {
                ctime = DateTime.Now,
                step = 6,
                mgr_id = mg.mgr_id,
                remark = "任务取消"
            });

            t.status = 6;

            db.SubmitChanges();
            return new XResp();
        }
    }
}
