using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws
{
    public class set_sell : xmg
    {
        [ParmsAttr(min = 1)]
        public int id { get; set; }
        /// <summary>
        /// 1、上架
        /// 2、下架
        /// </summary>
        [ParmsAttr(name = "值")]
        public int st { get; set; }

        protected override XResp Execute()
        {
            var dr = db.x_draw.FirstOrDefault(o => o.draw_id == id);
            if (dr == null) throw new XExcep("0x0014");

            dr.sell = st == 1;

            db.SubmitChanges();

            return new XResp();
        }
    }
}
