using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using X.Core.Utility;

namespace X.App.Views.tools.taobao
{
    public class show : xview
    {
        public string no { get; set; }
        public string tp { get; set; }
        protected override string GetParmNames
        {
            get
            {
                return "no-tp";
            }
        }

        public override byte[] GetResponse()
        {
            InitView();

            var fp = Context.Server.MapPath("/upload/temp/" + no + ".png");

            var par = new ProcessStartInfo(Context.Server.MapPath("/bin/phantomjs.exe"));
            par.Arguments = Context.Server.MapPath("/bin/ph.js") + " http://" + Context.Request.Url.Authority + "/tools/taobao/crt-" + no + "-" + tp + "-1.html " + fp;
            par.CreateNoWindow = true;
            par.UseShellExecute = true;
            par.WindowStyle = ProcessWindowStyle.Hidden;
            var pr = new Process();
            pr.StartInfo = par;
            pr.Start();
            pr.WaitForExit();
            pr.Dispose();

            Context.Response.ContentType = "application/zip";
            Context.Response.Headers.Add("Content-Disposition", "attachment;filename=" + Context.Server.UrlEncode(no + ".zip"));

            var img = Image.FromFile(fp);
            var h = img.Height / 10;

            var ms = new MemoryStream();
            var s = new ZipOutputStream(ms);
            //s.Password = Tools.GetRandRom(6);
            s.SetLevel(6);

            var t = 1;
            var p = new Bitmap(img.Width, h);
            var g = Graphics.FromImage(p);
            for (var i = 0; i <= img.Height - h; i += h, t++)
            {
                g.DrawImage(img, new Rectangle(0, 0, img.Width, h), new Rectangle(0, i, p.Width, h), GraphicsUnit.Pixel);
                var fn = t.ToString("00") + ".jpg";
                var ent = new ZipEntry(fn);
                ent.DateTime = DateTime.Now;
                s.PutNextEntry(ent);
                p.Save(s, ImageFormat.Jpeg);
            }
            s.Close();
            ms.Close();

            img.Dispose();
            File.Delete(fp);

            return ms.ToArray();
        }

    }
}
