using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws
{
    public class save : xmg
    {
        public int id { get; set; }
        public int st { get; set; }

        #region 第一步
        public string topic { get; set; }
        public int proj { get; set; }
        public int cate { get; set; }
        public string no { get; set; }
        public decimal price { get; set; }
        public int sales { get; set; }
        public int fan { get; set; }
        public int sort { get; set; }
        #endregion

        #region 第三步
        public string cover { get; set; }
        public int frame { get; set; }
        public int style { get; set; }
        public int layct { get; set; }
        public decimal l { get; set; }
        public decimal w { get; set; }
        public decimal cost1 { get; set; }
        public decimal cost2 { get; set; }
        public string attr { get; set; }
        public decimal zarea { get; set; }
        #endregion

        #region 第四步
        public string bd { get; set; }
        public string jg { get; set; }
        public string jp { get; set; }
        public string dq { get; set; }
        #endregion

        protected override string PowerCode => id == 0 ? "E010501" : "E010504";

        protected override XResp Execute()
        {
            x_draw d = null;
            if (id > 0) d = db.x_draw.FirstOrDefault(o => o.draw_id == id);
            if (d == null) d = new x_draw() { ctime = DateTime.Now };

            if (st == 1)
            {
                d.topic = topic;
                d.no = no;
                d.sell = false;
                d.sales = sales;
                d.price = price;
                d.no2 = fan;
                d.cate = cate;
                d.sort = sort;
                if (proj > 0)
                {
                    var p = db.x_project.FirstOrDefault(o => o.project_id == proj);
                    if (p != null) d.no = p.no;
                }
            }
            else if (st == 3)
            {
                d.attrs = attr;
                d.cover = cover;
                d.style = style;
                d.@struct = frame;
                d.open = l;
                d.depth = w;
                d.bprice = cost1 + "万~" + cost2 + "万";
                d.bprice1 = cost1;
                d.bprice2 = cost2;
                d.layct = layct;
                d.zarea = zarea;
                d.jarea = l * w;

                var ls = new List<x_draw.lay>();
                List<x_draw.lay> ols = null;
                if (!string.IsNullOrEmpty(d.layers)) ols = Serialize.FromJson<List<x_draw.lay>>(d.layers);
                if (ols == null) ols = new List<x_draw.lay>();

                for (var i = 1; i <= layct; i++)
                {
                    var l = ols.FirstOrDefault(o => o.id == i);
                    if (l == null) l = new x_draw.lay() { id = i };
                    if (l.height == 0) l.height = (decimal)3.3;
                    ls.Add(l);
                }

                if (d.attrs.Contains("[6]"))
                {
                    var l = ols.FirstOrDefault(o => o.id == 11);
                    if (l == null) l = new x_draw.lay() { id = 11 };
                    if (l.height == 0) l.height = 3;
                    ls.Insert(0, l);
                }

                if (d.attrs.Contains("[5]"))
                {
                    var l = ols.FirstOrDefault(o => o.id == 10);
                    if (l == null) l = new x_draw.lay() { id = 10 };
                    if (l.height == 0) l.height = 3;
                    ls.Add(l);
                }
                d.layers = Serialize.ToJson(ls);
            }
            else if (st == 4)
            {
                d.pbuild = bd;
                d.peleric = dq;
                d.pdrain = jp;
                d.pstruct = jg;
            }

            d.mtime = DateTime.Now;

            if (d.draw_id == 0)
                db.x_draw.InsertOnSubmit(d);
            db.SubmitChanges();

            return new XResp() { msg = d.draw_id + "" };
        }
    }
}
