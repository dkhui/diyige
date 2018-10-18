using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws.file
{
    public class del : xmg
    {
        public int id { get; set; }
        protected override string PowerCode => "E01050406";
        protected override XResp Execute()
        {
            var f = db.x_draw_file.FirstOrDefault(o => o.draw_file_id == id);
            if (f == null) throw new XExcep("0x0032");

            try
            {
                if (!string.IsNullOrEmpty(f.file)) File.Delete(Context.Server.MapPath(f.file.Replace("http://" + cfg.domain, "")));
                foreach (var p in f.prints?.Split(',')) File.Delete(Context.Server.MapPath(p.Replace("http://" + cfg.domain, "")));
            }
            catch { }

            db.x_draw_file.DeleteOnSubmit(f);
            db.SubmitChanges();

            return new XResp();
        }
    }
}
