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
    public class delbat : xmg
    {
        public int bat { get; set; }
        protected override int powercode
        {
            get
            {
                return 1;
            }
        }

        protected override XResp Execute()
        {
            var q = from s in DB.x_secode
                    where s.batch == bat
                    select s;

            if (q.Count() == 0) throw new XExcep("T批次不存在");

            DB.x_secode.DeleteAllOnSubmit(q);
            SubmitDBChanges();

            return new XResp();
        }

    }
}
