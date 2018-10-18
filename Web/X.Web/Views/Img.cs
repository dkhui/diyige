using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace X.Web.Views
{
    public abstract class Img : View
    {
        public int m { get; set; }
        public int w { get; set; }
        public int h { get; set; }

        string url = "";//访问路径
        string src = "";//原图
        string wimg = "";//目标图
        string ex = "";//后辍名

        protected override string GetParmNames
        {
            get
            {
                return "m-w-h";
            }
        }

        protected abstract string[] getExts();
        protected abstract List<wm> getWmlist();
        protected virtual bool showSrc { get { return false; } }

        public override byte[] GetResponse()
        {
            GetPageParms();

            url = Context.Request.RawUrl.Replace("_" + w, "").Replace("_" + h, "").Replace("_x_", "?").Split('?')[0];
            if (!showSrc && m == 0) throw new XExcep("T404，文件不存在");

            ex = url.Substring(url.LastIndexOf('.') + 1).Split('_')[0];
            if (!getExts().Contains(ex)) Context.Response.Redirect(url);

            src = Context.Server.MapPath(url.Split('_')[0]);
            if (!File.Exists(src)) throw new XExcep("T404，文件不存在");

            wimg = src.Replace("." + ex, "") + "_" + m + "." + ex;
            if (!File.Exists(wimg)) SetWm(wimg);

            var surl = wimg;
            if (w + h > 0) surl = src.Replace("." + ex, "") + "_" + m + "_" + w + "_" + h + "." + ex;
            if (!File.Exists(surl)) Resize(surl);

            Context.Response.ContentType = "image/" + ex;

            return File.ReadAllBytes(surl);
        }

        /// <summary>
        /// 设置水印
        /// </summary>
        /// <param name="fn"></param>
        void SetWm(string fn)
        {
            var wms = getWmlist();

            if (url.Contains("-none-") || m == 0) wms.Clear();

            var img = Image.FromFile(src);
            var g = Graphics.FromImage(img);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            foreach (var w in wms)
            {
                var m = new wo();
                try
                {
                    if (w.tp == 1)
                    {
                        var c = w.src.Split('|');
                        m.cot = c[0];
                        m.size = g.MeasureString(c[0], w.getFont());
                    }
                    else if (w.tp == 2)
                    {
                        var p = Image.FromFile(Context.Server.MapPath(w.src));
                        m.size = new SizeF(p.Width, p.Height);
                        m.cot = p.Clone();
                        p.Dispose();
                    }
                }
                catch { continue; }
                var pd = 10;
                var mg = 100;

                var s = new Size();
                if (img.Width < 400)
                {
                    s.Width = (int)(img.Width / 400.0 * w.size.Width);
                    s.Height = (int)(img.Width / 400.0 * w.size.Height);
                }
                else
                {
                    s.Width = img.Width / 6;
                    var rt = s.Width / (float)w.size.Width;
                    s.Height = (int)(w.size.Height * rt);
                    //s.Width = (int)(400.0 / img.Width * m.size.Width);
                    //s.Height = (int)(400.0 / img.Width * m.size.Height);
                }

                if (s.Width < w.size.Width || s.Height < w.size.Height) s = w.size;

                var pos = new PointF();
                switch (w.md)
                {
                    case 1:
                        pos.X = (img.Width - s.Width) / 2.0f;
                        pos.Y = (img.Height - s.Height) / 2.0f;
                        pos.X -= (s.Width + mg) * 10;
                        pos.Y -= (s.Height + mg) * 10;
                        break;
                    case 2:
                        pos.X = (img.Width - s.Width) / 2.0f;
                        pos.Y = (img.Height - s.Height) / 2.0f;
                        break;
                    case 3:
                        pos.X = pos.Y = pd;
                        break;
                    case 4:
                        pos.X = img.Width - s.Width - pd;
                        pos.Y = pd;
                        break;
                    case 5:
                        pos.X = pd;
                        pos.Y = img.Height - s.Height - pd;
                        break;
                    case 6:
                        pos.X = img.Width - s.Width - pd;
                        pos.Y = img.Height - s.Height - pd;
                        break;
                }

                if (w.md == 1)
                {
                    for (var x = pos.X; x <= img.Width; x += s.Width + mg)
                    {
                        for (var y = pos.Y; y <= img.Height; y += s.Height + mg)
                        {
                            if (w.tp == 1) g.DrawString(m.cot + "", w.getFont(), new SolidBrush(w.getColor()), x, y);
                            else g.DrawImage(m.cot as Image, new Rectangle((int)x, (int)y, s.Width, s.Height), new Rectangle(0, 0, (int)m.size.Width, (int)m.size.Height), GraphicsUnit.Pixel);
                        }
                    }
                }
                else
                {
                    if (w.tp == 1) g.DrawString(m.cot + "", w.getFont(), new SolidBrush(w.getColor()), pos.X, pos.Y);
                    else g.DrawImage(m.cot as Image, new Rectangle((int)pos.X, (int)pos.Y, (int)m.size.Width, (int)m.size.Height), new Rectangle(0, 0, (int)m.size.Width, (int)m.size.Height), GraphicsUnit.Pixel);
                }

            }
            g.Dispose();

            var eps = new EncoderParameters(1);
            eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);

            img.Save(fn, ImageCodecInfo.GetImageDecoders().FirstOrDefault(o => o.FormatDescription.Equals("JPEG")), eps);
            img.Dispose();
        }

        /// <summary>
        /// 缩略图
        /// </summary>
        /// <param name="fn"></param>
        void Resize(string fn)
        {
            var img = Image.FromFile(wimg);
            int iw = w, ih = h;
            if (w == 0) iw = (int)(img.Width * ((float)h) / img.Height);
            else if (h == 0) ih = (int)(img.Height * ((float)w / img.Width));
            else { }

            var bmp = new Bitmap(iw, ih);
            var g = Graphics.FromImage(bmp);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            img.Dispose();
            g.Dispose();

            if (ex == "jpg" || ex == "jpeg")
            {
                var eps = new EncoderParameters(1);
                eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                var cdi = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatDescription.Equals("JPEG"));
                bmp.Save(fn, cdi, eps);
            }
            else bmp.Save(fn);
            bmp.Dispose();
        }

        class wo
        {
            public SizeF size { get; set; }
            public object cot { get; set; }
        }

        protected class wm
        {
            /// <summary>
            /// 图片为地址
            /// 文字格式：内容|字体|大小|颜色
            /// </summary>
            public string src { get; set; }
            /// <summary>
            /// 1、文字
            /// 2、图片
            /// </summary>
            public int tp { get; set; }
            /// <summary>
            /// 1、平铺
            /// 2、居中
            /// 3、左上
            /// 4、右上
            /// 5、左下
            /// 6、右下
            /// </summary>
            public int md { get; set; }
            /// <summary>
            /// 在400*300 图像里占多大
            /// </summary>
            public Size size { get; set; }

            public Font getFont()
            {
                if (tp == 2) return new Font("黑体", 20);
                var c = src.Split('|');
                return new Font(c[1], float.Parse(c[2]));
            }
            public Color getColor()
            {
                if (tp == 2) return Color.White;
                var c = src.Split('|');
                return c[3][0] == '#' ? ColorTranslator.FromHtml(c[3]) : Color.FromName(c[3]);
            }
        }

        protected override Dictionary<string, string> GetDataList(string cd, string up)
        {
            throw new NotImplementedException();
        }
    }
}
