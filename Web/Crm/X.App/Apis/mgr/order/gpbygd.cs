using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.order
{
    /// <summary>
    /// 按商品统计收入
    /// </summary>
    public class gpbygd : xmg
    {
        public long apid { get; set; }
        protected override XResp Execute()
        {
            var ap = db.gt_app.FirstOrDefault(o => o.app_id == apid);
            if (ap == null) throw new XExcep("T应用不存在");

            var list = new List<object>();

            foreach (var a in ap.gt_app_goods)
            {
                list.Add(new
                {
                    name = a.name,
                    count = ap.gt_order.Count(o => o.goods_id == a.app_goods_id),
                    sum = ap.gt_order.Where(o => o.goods_id == a.app_goods_id).Sum(o => o.paid)
                });
            }
            return new XList() { items = list };
        }
    }
}
