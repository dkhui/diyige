using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web.Com;

namespace X.App.Apis.pc.draws
{
    public class sync : xmg
    {
        protected override XResp Execute()
        {
            var list = db.t_aci_img.Take(30).ToList();
            var ser = new Com.PHP.Serializer();

            foreach (var t in list)
            {
                x_draw d = null;
                d = db.x_draw.FirstOrDefault(o => o.no == t.img_no);
                if (d == null) d = new x_draw();

                db.t_aci_img.DeleteOnSubmit(t);

                try
                {
                    var pgs = Serialize.FromJson<Dictionary<int, List<string>>>(Serialize.ToJson(ser.Deserialize(t.imglist)));
                    d.x_draw_img.Clear();
                    foreach (var p in pgs)
                        foreach (var i in p.Value)
                        {
                            var u = Tools.DownImage("http://www.bieshu.com/" + i, "-none-");
                            if (string.IsNullOrEmpty(u)) continue;
                            d.x_draw_img.Add(new x_draw_img()
                            {
                                name = "",
                                type = p.Key,
                                mgr_id = mg.mgr_id,
                                url = "http://" + cfg.domain + u + "_x_0"
                            });
                        }
                }
                catch
                {
                    continue;
                }

                var bd = d.x_draw_file.FirstOrDefault(o => o.type == 1);
                if (bd == null) bd = new x_draw_file() { mgr_id = mg.mgr_id, ctime = DateTime.Now, type = 1, name = "建筑" };
                bd.cot = t.blueprint;
                d.pbuild = bd.cot;
                if (d.x_draw_img.Count() > 0) d.cover = d.x_draw_img.First().url;

                if (bd.draw_file_id == 0) d.x_draw_file.Add(bd);

                var st = d.x_draw_file.FirstOrDefault(o => o.type == 2);
                if (st == null) st = new x_draw_file() { mgr_id = mg.mgr_id, ctime = DateTime.Now, type = 2, name = "结构" };
                st.cot = t.structure;
                d.pstruct = st.cot;
                if (st.draw_file_id == 0) d.x_draw_file.Add(st);

                var ps = d.x_draw_file.FirstOrDefault(o => o.type == 3);
                if (ps == null) ps = new x_draw_file() { mgr_id = mg.mgr_id, ctime = DateTime.Now, type = 3, name = "给排水" };
                ps.cot = t.drain;
                d.pdrain = ps.cot;
                if (ps.draw_file_id == 0) d.x_draw_file.Add(ps);

                var dq = d.x_draw_file.FirstOrDefault(o => o.type == 4);
                if (dq == null) dq = new x_draw_file() { mgr_id = mg.mgr_id, ctime = DateTime.Now, type = 4, name = "电气" };
                dq.cot = t.electric;
                d.peleric = dq.cot;
                if (dq.draw_file_id == 0) d.x_draw_file.Add(dq);

                try
                {
                    var des = Serialize.FromJson<Dictionary<string, x_draw.lay>>(Serialize.ToJson(ser.Deserialize(t.design)));
                    var lays = new List<x_draw.lay>();
                    foreach (var l in des) { if (string.IsNullOrEmpty(l.Key) || l.Value == null) continue; l.Value.id = int.Parse(l.Key); lays.Add(l.Value); }
                    d.layers = Serialize.ToJson(lays.OrderBy(o => o.id));
                    d.layct = lays.Count();
                }
                catch
                {
                    continue;
                }

                d.alias = "";
                d.attrs = "";
                d.bprice = t.cost;
                d.cate = 1;
                d.ctime = DateTime.Now;
                if (d.sales == 0) d.sales = t.sales;
                d.depth = t.depth;
                d.jarea = t.jarea;
                d.mtime = DateTime.Now;
                d.no = t.img_no;
                d.open = t.openroom;
                d.pbuild = t.blueprint;
                d.pdrain = t.drain;
                d.peleric = t.electric;
                d.price = t.price;
                d.pstruct = t.structure;
                d.@struct = int.Parse(t.framework);
                d.style = int.Parse(t.style);
                d.topic = t.title;
                d.zarea = t.zarea;

                if (d.draw_id == 0) db.x_draw.InsertOnSubmit(d);
            }

            db.SubmitChanges();

            return new XResp();
        }
        class img
        {
            public string url { get; set; }
            public int type { get; set; }
        }
    }
}
