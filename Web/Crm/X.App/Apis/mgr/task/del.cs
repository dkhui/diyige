using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.proj
{
    public class del : xmg
    {

        [ParmsAttr(name = "项目ID")]
        public int id { get; set; }
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("T项目不存在");

            var od = db.x_order.FirstOrDefault(o => o.pid == id);
            if (od != null)
            {
                db.x_order_pay.DeleteAllOnSubmit(od.x_order_pay);
                db.x_order.DeleteOnSubmit(od);
            }

            db.x_project_log.DeleteAllOnSubmit(p.x_project_log);
            db.x_project.DeleteOnSubmit(p);

            db.SubmitChanges();

            return new XResp();
        }
    }
}
