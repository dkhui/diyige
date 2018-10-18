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
    public class copy : xmg
    {
        public int id { get; set; }
        protected override string PowerCode => "E010506";
        protected override XResp Execute()
        {
            var dr = db.x_draw.FirstOrDefault(o => o.draw_id == id);
            if (dr == null) throw new XExcep("T图纸不存在");
            if (dr.mirror == true) throw new XExcep("T镜像图纸不能再次镜像");

            var nd = new x_draw()
            {
                alias = dr.alias,
                mirror = true,
                aperson = dr.aperson,
                astatus = dr.astatus,
                atime = dr.atime,
                attrs = dr.attrs,
                bprice = dr.bprice,
                bprice1 = dr.bprice1,
                bprice2 = dr.bprice2,
                cate = dr.cate,
                cover = "",
                sell = false,
                ctime = dr.ctime,
                depth = dr.depth,
                dutier = dr.dutier,
                images = dr.images,
                jarea = dr.jarea,
                layct = dr.layct,
                layers = dr.layers,
                mtime = dr.mtime,
                no = dr.no,
                no2 = dr.no2,
                open = dr.open,
                pbuild = dr.pbuild,
                pdrain = dr.pdrain,
                peleric = dr.pdrain,
                pop = dr.pop,
                price = dr.price,
                pstruct = dr.pstruct,
                sales = dr.sales,
                sort = dr.sort,
                @struct = dr.@struct,
                style = dr.style,
                topic = dr.topic + " 镜像",
                zarea = dr.zarea
            };

            foreach (var f in dr.x_draw_file) nd.x_draw_file.Add(new x_draw_file()
            {
                aperson = f.aperson,
                astatus = f.astatus,
                atime = f.atime,
                cot = f.cot,
                ctime = f.ctime,
                file = "",
                mgr_id = f.mgr_id,
                name = f.name,
                prints = "",
                remark = f.remark,
                type = f.type
            });

            //foreach (var p in dr.x_draw_img) nd.x_draw_img.Add(new x_draw_img()
            //{
            //    mgr_id = p.mgr_id,
            //    name = p.name,
            //    remark = p.remark,
            //    type = p.type,
            //    url = p.url
            //});

            db.x_draw.InsertOnSubmit(nd);

            db.SubmitChanges();

            return new XResp() { msg = nd.draw_id + "" };
        }
    }
}
