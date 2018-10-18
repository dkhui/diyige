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
    public class sub1 : sub
    {
        [ParmsAttr(name = "楼层方案", req = true)]
        public string lays { get; set; }

        protected override void Submit()
        {
            var ls = Serialize.FromJson<List<x_draw.lay>>(Context.Server.HtmlDecode(lays));
            if (ls == null) throw new XExcep("T楼层信息错误！");

            var imgs = dr.x_draw_img.Where(o => o.type == 1);
            db.x_draw_img.DeleteAllOnSubmit(imgs);

            foreach (var l in ls)
            {
                dr.x_draw_img.Add(new x_draw_img()
                {
                    name = l.title + "平面图",
                    type = 1,
                    url = l.pic
                });
            }
            dr.layers = Serialize.ToJson(ls);
        }
    }
}
