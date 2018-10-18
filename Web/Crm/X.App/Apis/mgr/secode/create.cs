using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;
using X.Data;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.mgr.secode
{
    /// <summary>
    /// 删除用户
    /// </summary>
    public class create : xmg
    {
        [ParmsAttr(name = "批号", min = 1)]
        public int batch { get; set; }
        [ParmsAttr(name = "数量", max = 99999999, min = 1)]
        public int count { get; set; }

        protected override int powercode
        {
            get
            {
                return 1;
            }
        }


        protected override XResp Execute()
        {
            var list = new List<x_secode>();
            for (var i = 1; i <= count; i++)
            {
                var no = batch + i.ToString("00000000");
                var sc = new x_secode()
                {
                    batch = batch,
                    scount = 0,
                    incode = i,
                    outcode = Secret.MD5(no + "%c&iz4VV2XIzItlK")
                };
                list.Add(sc);
            }

            DB.x_secode.InsertAllOnSubmit(list);
            SubmitDBChanges();

            return new XResp();
        }

    }
}
