using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws.file
{
    public class get : xmg
    {
        protected override string PowerCode => "E010508";
        public string no { get; set; }
        public int no2 { get; set; }
        public int m1 { get; set; }
        /// <summary>
        /// 1、pdf
        /// 2、cad
        /// 3、pic
        /// </summary>
        public int tp { get; set; }
        protected override XResp Execute()
        {
            var q = db.x_draw.Where(o => o.no == no);
            if (no2 > 0) q = q.Where(o => o.no2 == no2);
            if (m1 > 0) q = q.Where(o => o.mirror == true);

            var dr = q.FirstOrDefault(o => o.no == no);
            if (dr == null) throw new XExcep("T图纸不存在");

            var r = new XList();
            if (tp == 1)
                r.items = dr.x_draw_file.Where(o => o.type > 2 && o.prints != null && o.prints != "").ToDictionary(k => db.GetDictName("draw.file.type", k.type) + k.draw_file_id, v => v.prints.Split(','));
            else if (tp == 2)
                r.items = dr.x_draw_file.Where(o => o.type > 2 && o.file != null && o.file != "").ToDictionary(k => db.GetDictName("draw.file.type", k.type) + k.draw_file_id, v => v.file.Split(','));
            else if (tp == 3)
                r.items = dr.x_draw_file.Where(o => o.type <= 2 && o.prints != null && o.prints != "").ToDictionary(k => db.GetDictName("draw.file.type", k.type) + k.draw_file_id, v => v.prints.Split(','));
            return r;
        }
    }
}
