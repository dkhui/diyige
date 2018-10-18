using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws.img
{
    public class list : xmg
    {
        [ParmsAttr(name = "图纸id", min = 1)]
        public long dr_id { get; set; }
        [ParmsAttr(def = 1)]
        public int page { get; set; }
        [ParmsAttr(def = 90)]
        public int limit { get; set; }
        public int tp { get; set; }
        
        protected override XResp Execute()
        {
            var dr = db.x_draw.FirstOrDefault(o => o.draw_id == dr_id);
            if (dr == null) throw new XExcep("T图纸不存在");

            var q = from p in dr.x_draw_img
                    select p;

            if (tp > 0) q = q.Where(o => o.type == tp);
            else if (tp == 999) q = q.Where(o => o.type == null || o.type == 0);

            var r = new XList();
            r.page = page;
            r.count = q.Count();
            r.items = q.Skip((page - 1) * limit).Take(limit).Select(o => new
            {
                id = o.draw_img_id,
                o.name,
                cate = db.GetDictName("draw.img.type", o.type),
                o.url
            }).ToList();

            return r;
        }
    }
}
