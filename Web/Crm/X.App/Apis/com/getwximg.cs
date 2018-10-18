using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using X.App.Com;
using X.Web.Com;

namespace X.App.Apis.com
{
    public class getwximg : xapi
    {
        public string svr_id { get; set; }
        protected override XResp Execute()
        {
            var tk = Wx.GetToken(cfg.wx.appid, cfg.wx.secret);
            var url = Wx.Media.DownImage(tk, svr_id);

            var img = Image.FromFile(Context.Server.MapPath(url));
            var bmp = new Bitmap(img);
            img.Dispose();
            var wm = Image.FromFile(Context.Server.MapPath("/res/img/book/wm.png"));
            var g = Graphics.FromImage(bmp);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            for (var x = -wm.Width / 2; x < bmp.Width; x += 200)
                for (var y = -wm.Height / 2; y < bmp.Height; y += 200) g.DrawImage(wm, x, y);

            wm.Dispose();

            var rt = (bmp.Width / 400.0f);
            wm = Image.FromFile(Context.Server.MapPath("/res/img/book/wm1.png"));
            g.DrawImage(wm, new Rectangle(bmp.Width - (int)(110 * rt) - 5, bmp.Height - (int)(31 * rt) - 5, (int)(110 * rt), (int)(31 * rt)), new Rectangle(0, 0, wm.Width, wm.Height), GraphicsUnit.Pixel);
            wm.Dispose();
            //var rt = (bmp.Width / 400.0f);
            //var fz = 14 * rt;
            //if (fz < 12) fz = 12;

            //var size = g.MeasureString("别墅工场(bieshu.com)", new Font("黑体", fz));
            //g.DrawString("别墅工场(bieshu.com)", new Font("黑体", fz), Brushes.White, bmp.Width - size.Width - 5 * rt, bmp.Height - size.Height - 5 * rt);

            var eps = new EncoderParameters(1);
            eps.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            var cdi = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatDescription.Equals("JPEG"));
            bmp.Save(Context.Server.MapPath(url), cdi, eps);

            bmp.Dispose();

            return new XResp() { msg = url };
        }
    }
}
