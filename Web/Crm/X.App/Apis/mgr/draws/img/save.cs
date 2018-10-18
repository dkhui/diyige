using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws.img
{
    public class save : xmg
    {
        [ParmsAttr(name = "图片ID")]
        public long pid { get; set; }
        [ParmsAttr(name = "图纸ID", req = true)]
        public long did { get; set; }
        public string name { get; set; }

        [ParmsAttr(name = "图片地址", req = true)]
        public string url { get; set; }
        [ParmsAttr(name = "图片类型")]
        public int tp { get; set; }

        protected override XResp Execute()
        {
            var dr = db.x_draw.FirstOrDefault(o => o.draw_id == did);
            if (dr == null) throw new XExcep("T图纸不存在");

            x_draw_img img = null;
            if (pid > 0) img = dr.x_draw_img.FirstOrDefault(o => o.draw_img_id == pid);
            if (img == null) img = new x_draw_img();

            if (!string.IsNullOrEmpty(url)) img.url = url;
            if (!string.IsNullOrEmpty(name)) img.name = name;
            img.type = tp;

            if (img.draw_img_id == 0) dr.x_draw_img.Add(img);

            db.SubmitChanges();

            return new XResp();
        }

    }
}
