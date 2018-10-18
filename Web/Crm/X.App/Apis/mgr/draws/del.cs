using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws
{
    public class del : xmg
    {
        [ParmsAttr(min = 1)]
        public int id { get; set; }

        protected override string PowerCode => "E010505";
        protected override XResp Execute()
        {
            var dr = db.x_draw.FirstOrDefault(o => o.draw_id == id);
            if (dr == null) throw new XExcep("0x0014");

            db.x_draw_file.DeleteAllOnSubmit(dr.x_draw_file);
            db.x_draw_img.DeleteAllOnSubmit(dr.x_draw_img);
            db.x_draw_reply.DeleteAllOnSubmit(dr.x_draw_reply);
            db.x_user_fav.DeleteAllOnSubmit(dr.x_user_fav);
            db.x_draw.DeleteOnSubmit(dr);

            db.SubmitChanges();

            return new XResp();
        }
    }
}
