using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws.img
{
    public class del : xmg
    {
        public string ids { get; set; }

        protected override string PowerCode => "E01050402";
        protected override XResp Execute()
        {
            var fs = db.x_draw_img.Where(o => ids.Split(',').Contains(o.draw_img_id + ""));

            foreach (var f in fs)
            {
                try
                {
                    if (!string.IsNullOrEmpty(f.url) && db.x_draw_img.Count(o => o.url == f.url) == 1)
                    {
                        var fi = new FileInfo(Context.Server.MapPath(f.url.Replace("http://" + cfg.domain, "")));
                        var files = fi.Directory.GetFiles(fi.Name.Split('.')[0] + "*");
                        foreach (var file in files) file.Delete();
                    }
                }
                catch { }
                db.x_draw_img.DeleteOnSubmit(f);
            }
            db.SubmitChanges();

            return new XResp();
        }
    }
}
