using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using X.Core.Utility;
using System.Web;

namespace X.App.Apis.com
{
    public class imgX
    {

        public static void delImg(string filepath)
        {
            if (File.Exists(filepath))
            {
                try
                {
                    File.Delete(filepath);
                }
                catch { }
            }
        }


        public static string saveFilesImg(string imgbase64)
        {
            string path = HttpContext.Current.Server.MapPath("\\");
            string r = saveimg(path + "upload\\files\\", imgbase64);
            return r.Replace(path, "");
        }

        /// <summary>
        /// 人脉通图片保存,保存在目录下面rmt文件夹下
        /// </summary>
        /// <param name="imgbase64"></param>
        /// <returns></returns>
        public static string saveRmtImg(string imgbase64)
        {
            string path = HttpContext.Current.Server.MapPath("\\");
            string r = saveimg(path+ "upload\\rmt\\", imgbase64);
            return r.Replace(path, "");
        }

        public static string saveimg(string path,string imgbase64)
        {
           
            byte[] bf = Convert.FromBase64String(imgbase64);
            return saveimg(path, bf);
        }

        public static string saveimg(string path,byte[] buff)
        {
            path += DateTime.Now.ToString("yyyy")+"\\"+ DateTime.Now.ToString("MM")+"\\"+ DateTime.Now.ToString("dd");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string n = Guid.NewGuid().ToString("N")+".png";
            path = path + "\\" + n;

            FileStream file = new FileStream(path, FileMode.Create);
            file.Write(buff, 0, buff.Length);
            file.Flush();
            file.Close();
            return path;
        }

        public static Bitmap byeToimg(byte[] buff)
        {
            MemoryStream ms = new MemoryStream(buff);
            Bitmap bmpt = new Bitmap(ms);
            return bmpt;
        }

        public static byte[] imgTobye(Image img)
        {
            //将Image转换成流数据，并保存为byte[]
            MemoryStream mstream = new MemoryStream();
            img.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] byData = new Byte[mstream.Length];
            mstream.Position = 0;
            mstream.Read(byData, 0, byData.Length);
            mstream.Close();
            return byData;
        }



    }
}
