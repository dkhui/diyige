using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.pc.draws.img
{
    public class tocate : xmg
    {
        [ParmsAttr(name = "图片IDS", req = true)]
        public string ids { get; set; }
        [ParmsAttr(name = "分类", req = true)]
        public int tp { get; set; }
        protected override string PowerCode => "E01050402";
        protected override XResp Execute()
        {
            var fs = db.x_draw_img.Where(o => ids.Split(',').Contains(o.draw_img_id + ""));
            foreach (var f in fs) f.type = tp;
            db.SubmitChanges();
            return new XResp();
        }

    }
}
