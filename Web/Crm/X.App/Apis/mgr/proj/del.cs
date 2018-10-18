using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.proj
{
    public class del : xmg
    {

        [ParmsAttr(name = "项目ID")]
        public int id { get; set; }

        protected override string PowerCode => "E010212";
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("T项目不存在");

            var od = db.x_order.FirstOrDefault(o => o.pid == id);
            if (od != null)
            {
                db.x_order_pay.DeleteAllOnSubmit(od.x_order_pay);//删除支付记录
                db.x_order.DeleteOnSubmit(od);//删除订单
            }

            var dr = db.x_draw.FirstOrDefault(o => o.no == p.imgno);
            if (dr != null)
            {
                db.x_draw_file.DeleteAllOnSubmit(dr.x_draw_file);//删除图纸文件
                db.x_draw_img.DeleteAllOnSubmit(dr.x_draw_img);//删除图纸图片
                db.x_draw_reply.DeleteAllOnSubmit(dr.x_draw_reply);//删除图纸回复
                db.x_user_fav.DeleteAllOnSubmit(dr.x_user_fav);//删除图纸收藏
                db.x_draw.DeleteOnSubmit(dr);//删除图纸
            }

            db.x_project_log.DeleteAllOnSubmit(p.x_project_log);//删除日志

            var tls = db.x_task_log.Where(o => p.x_task.Select(t => t.task_id).Contains(o.task_id.Value));
            db.x_task_log.DeleteAllOnSubmit(tls);//删除任务日志
            db.x_task.DeleteAllOnSubmit(p.x_task);//删除日志

            db.x_project.DeleteOnSubmit(p);//删除项目

            db.SubmitChanges();

            return new XResp();
        }
    }
}
