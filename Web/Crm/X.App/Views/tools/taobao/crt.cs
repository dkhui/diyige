using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using X.App.Com;
using X.Core.Utility;
using X.Web;

namespace X.App.Views.tools.taobao
{
    public class crt : xview
    {
        protected override string GetParmNames => "no-tp-m";

        string cn_nums = "一二三四五六七八九十";
        string en_nums = "ONE TWO THREE FOUR FIVE SIX SEVEN EIGHT NINE TEN";

        [ParmsAttr(name = "图纸编号", req = true)]
        public string no { get; set; }
        public string tp { get; set; }

        protected override void InitDict()
        {
            base.InitDict();

            var imgs = db.x_draw.Where(o => o.no == no).ToList();
            if (imgs.Count == 0) throw new XExcep("T图纸编号不存在");

            dict.Add("imgs", imgs);

            dict.Add("l", imgs[0]);
            dict.Add("ps", (imgs[0].pbuild + imgs[0].pstruct + imgs[0].pdrain + imgs[0].peleric).Split('，').Count());

        }
        public string getNo2(int n)
        {
            return db.GetDictName("draw.scheme", n);
        }
        public string getNo(int n, string lg)
        {
            if (n < 1 || n > 10) return "";
            return lg == "en" ? en_nums.Split(' ')[n - 1] : cn_nums[n - 1] + "";
        }

        public object getLays(string lay_cfg)
        {
            var lays = Serialize.FromJson<List<x_draw.lay>>(lay_cfg);
            lays.Reverse();
            return lays;
        }

        public object getImages(x_draw dr, int tp)
        {
            var list = dr.x_draw_img;
            return list.Where(o => o.type == tp).Select(o => o.url).ToArray();
        }

        public string getStyle(object id)
        {
            return db.GetDictName("draw.style", id);
        }

        public override string GetTplFile()
        {
            if (tp == "") tp = "1";
            return "tools/taobao/p" + tp;
        }
        class lay
        {
            public string title { get; set; }
            public string text { get; set; }
            public decimal height { get; set; }
        }
        class img
        {
            public string url { get; set; }
            public int type { get; set; }
        }
    }
}
