using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws.lay
{
    public class save : xmg
    {
        public int did { get; set; }
        public int id { get; set; }
        public string n { get; set; }
        public string c { get; set; }
        public string p { get; set; }
        public decimal h { get; set; }
        protected override string PowerCode => "E01050405";
        protected override XResp Execute()
        {
            var dr = db.x_draw.FirstOrDefault(o => o.draw_id == did);
            if (dr == null) throw new XExcep("0x0014");

            var ls = Serialize.FromJson<List<x_draw.lay>>(dr.layers);

            x_draw.lay l = null;
            if (ls == null) ls = new List<x_draw.lay>();

            if (id > 0) l = ls.FirstOrDefault(o => o.id == id);
            if (l == null) l = new x_draw.lay();

            l.title = n;
            l.pic = p;
            l.text = c;
            l.height = h;

            if (l.id == 0) { l.id = ls.Count() + 1; ls.Add(l); }

            dr.layers = Serialize.ToJson(ls);
            db.SubmitChanges();

            return new XResp();
        }
    }
}
