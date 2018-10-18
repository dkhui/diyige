using System.Linq;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.order
{
    public class del : xmg
    {
        public int id { get; set; }

        protected override string PowerCode => "E010408";
        protected override XResp Execute()
        {
            var od = db.x_order.FirstOrDefault(o => o.order_id == id);

            if (od == null) throw new XExcep("0x0024");

            db.x_order_pay.DeleteAllOnSubmit(od.x_order_pay);
            db.x_order.DeleteOnSubmit(od);

            db.SubmitChanges();

            return new XResp();
        }
    }
}
