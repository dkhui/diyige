using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.proj
{
    public class setup : xmg
    {
        [ParmsAttr(name = "项目ID")]
        public long id { get; set; }
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("T项目不存在");

            if (p.status != 1) throw new XExcep("T项目当前状态不能立项");

            var od = db.x_order.FirstOrDefault(o => o.pid == id);
            if (od == null) throw new XExcep("T当前项目有问题，找不到订单");

            if (od.x_order_pay.FirstOrDefault(o => o.type == 2) == null) throw new XExcep("T未收到项目定金！");

            var lg = new Com.x_project_log()
            {
                cot = "项目立项",
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                type = 2
            };
            p.x_project_log.Add(lg);

            p.status = 2;
            db.SubmitChanges();

            return new XResp();
        }
    }
}
