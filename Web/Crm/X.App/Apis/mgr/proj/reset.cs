using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.proj
{
    public class reset : xmg
    {
        [ParmsAttr(name = "项目ID", min = 1)]
        public long id { get; set; }
        protected override string PowerCode => "E010213";
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("T项目不存在");
            if (p.isstop != true) throw new XExcep("T项目不是中止状态！");

            p.isstop = false;

            p.x_project_log.Add(new Com.x_project_log()
            {
                cot = "项目重启",
                ctime = DateTime.Now,
                type = 11,
                mgr_id = mg.mgr_id
            });

            db.SubmitChanges();

            return new XResp();
        }
    }
}
