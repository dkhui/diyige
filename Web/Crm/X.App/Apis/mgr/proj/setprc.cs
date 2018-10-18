using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.proj
{
    public class setpr : xmg
    {
        public long id { get; set; }
        //public long tkid { get; set; }
        public decimal price { get; set; }
        public string remark { get; set; }

        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("T项目不存在");
            if (p.status > 1) throw new XExcep("T项目当前状态不能重新定价！！！");

            var od = db.x_order.FirstOrDefault(o => o.pid == id);
            if (od == null) throw new XExcep("T项目找不到对应的订单");

            var lg = new Com.x_project_log()
            {
                cot = "项目价格设定，此次价格为：" + price,
                ctime = DateTime.Now,
                type = 1,
                mgr_id = mg.mgr_id,
                remark = remark
            };

            od.payable = od.amount = price;
            od.paid = 0;

            //if (tkid > 0)
            //{
            //    var tk = od.x_user.x_user_ticket.FirstOrDefault(o => o.ticket_id == tkid);
            //    if (tk != null)
            //    {
            //        od.offam = tk.amount > 1 ? tk.amount : od.amount * (1 - tk.amount);
            //        od.offremark = "使用优惠券【" + tk.x_ticket.topic + "】，优惠金额：" + od.offam;
            //        od.payable = od.amount - od.offam;
            //        od.offtkid = tkid;
            //    }
            //    tk.status = 2;
            //}

            if (p.status == 0) p.status = 1;
            p.x_project_log.Add(lg);

            db.SubmitChanges();
            return new XResp();
        }
    }
}
