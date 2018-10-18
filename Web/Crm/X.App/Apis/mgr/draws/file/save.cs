using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws.file
{
    public class save : xmg
    {
        public int id { get; set; }
        [ParmsAttr(name = "图纸ID", req = true)]
        public int did { get; set; }
        public string name { get; set; }
        [ParmsAttr(name = "类型", min = 1)]
        public int type { get; set; }
        public string pdfs { get; set; }
        public string file { get; set; }
        public string cot { get; set; }
        public string remark { get; set; }
        protected override string PowerCode => "E01050406";
        protected override XResp Execute()
        {
            var dr = db.x_draw.FirstOrDefault(o => o.draw_id == did);
            if (dr == null) throw new XExcep("0x0014");

            x_draw_file f = null;
            if (id > 0) f = dr.x_draw_file.FirstOrDefault(o => o.draw_file_id == id);
            if (f == null) f = new x_draw_file() { ctime = DateTime.Now, draw_id = did };

            f.file = file;
            f.mgr_id = mg.mgr_id;
            if (!string.IsNullOrEmpty(name)) f.name = name;
            else if (string.IsNullOrEmpty(f.name)) f.name = db.GetDictName("draw.file.type", type);
            if (!string.IsNullOrEmpty(cot)) f.cot = cot;
            f.prints = pdfs;
            f.remark = remark;
            f.type = type;

            if (f.draw_file_id == 0) dr.x_draw_file.Add(f);

            db.SubmitChanges();
            return new XResp();
        }
    }
}
