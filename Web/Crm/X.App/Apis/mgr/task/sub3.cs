using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.task
{
    public class sub3 : sub
    {
        //[ParmsAttr(name = "正立面", req = true)]
        //public string zlm { get; set; }
        //[ParmsAttr(name = "背立面", req = true)]
        //public string blm { get; set; }

        protected override void Submit()
        {
            //var imgs = dr.x_draw_img.Where(o => o.type == 2 || o.type == 9);
            //db.x_draw_img.DeleteAllOnSubmit(imgs);
            //dr.x_draw_img.Add(new x_draw_img()
            //{
            //    name = "正立面",
            //    type = 2,
            //    url = zlm
            //});
            //dr.x_draw_img.Add(new x_draw_img()
            //{
            //    name = "背立面",
            //    type = 9,
            //    url = blm
            //});
            dr.pbuild = cot;
        }
    }
}
