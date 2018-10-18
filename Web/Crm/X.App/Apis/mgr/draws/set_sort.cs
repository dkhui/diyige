using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.draws
{
    public class set_sort : xmg
    {
        [ParmsAttr(min = 1)]
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ParmsAttr(name = "排序值")]
        public int val { get; set; }

        protected override XResp Execute()
        {
            var dr = db.x_draw.FirstOrDefault(o => o.draw_id == id);
            if (dr == null) throw new XExcep("0x0014");

            dr.sort = val;

            db.SubmitChanges();

            return new XResp();
        }
    }
}
