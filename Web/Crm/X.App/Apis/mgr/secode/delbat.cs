using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.secode
{
    /// <summary>
    /// 删除用户
    /// </summary>
    public class del : xmg
    {
        public int id { get; set; }
        protected override int powercode
        {
            get
            {
                return 1;
            }
        }

        protected override XResp Execute()
        {
            var se = DB.x_secode.FirstOrDefault(o => o.secode_id == id);
            if (se == null) throw new XExcep("T防伪码不存在");

            DB.x_secode.DeleteOnSubmit(se);
            SubmitDBChanges();

            return new XResp();
        }

    }
}
