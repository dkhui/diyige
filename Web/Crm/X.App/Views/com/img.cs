using System;
using System.Collections.Generic;
using System.Drawing;
using X.Web.Views;

namespace X.App.Views.com
{
    public class img : Img
    {
        protected override string[] getExts()
        {
            return "jpg|jpeg|png|bmp".Split('|');
        }

        protected override bool showSrc
        {
            get
            {
                return Context.Request.RawUrl.Contains("-none-");
            }
        }

        protected override List<wm> getWmlist()
        {
            var list = new List<wm>();
            list.Add(new wm()
            {
                md = 1,
                src = "/res/img/mgr/wmn.png",
                tp = 2,
                size = new Size(216, 149)
            });
            //list.Add(new wm()
            //{
            //    md = 6,
            //    src = "/res/img/mgr/wm1.png",
            //    tp = 2
            //});
            return list;
        }
    }
}
