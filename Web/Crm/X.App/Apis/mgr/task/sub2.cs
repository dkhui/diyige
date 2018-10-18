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
    public class sub2 : sub
    {
        public new string src { get; set; }
        [ParmsAttr(name = "效果图", req = true)]
        public string xyt { get; set; }
        public string lht { get; set; }
        public string tst { get; set; }
        public string re1 { get; set; }
        public string re2 { get; set; }
        public string re3 { get; set; }

        protected override void Submit()
        {
            var imgs = dr.x_draw_img.Where(o => o.type == 3);
            db.x_draw_img.DeleteAllOnSubmit(imgs);

            dr.x_draw_img.Clear();

            dr.cover = xyt.Split(',')[0];

            foreach (var x in xyt.Split(','))
            {
                dr.x_draw_img.Add(new x_draw_img()
                {
                    name = "小样图",
                    type = 3,
                    mgr_id = mg.mgr_id,
                    url = x,
                    remark = re1
                });
            }
        }
    }
}
